using System;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using Marten;
using Meetup.Domain;

namespace Meetup.Api
{
    public class MeetupAppService
    {
        private readonly IDocumentStore _documentStore;
        private readonly AddressValidator _addressValidator;
        private readonly IBus _bus;

        public MeetupAppService(IDocumentStore store, AddressValidator addressValidator, IBus bus)
        {
            _documentStore = store;
            _addressValidator = addressValidator;
            _bus = bus;
        }

        public Task Handle(object command) => command switch
        {
            Meetup.V1.Create cmd =>
                ExecuteTransaction(
                new MeetupAggregate(
                    new MeetupId(cmd.Id),
                    new MeetupTitle(cmd.Title),
                    new Location(_addressValidator, cmd.Location))),

            Meetup.V1.UpdateNumberOfSeats cmd =>
                ExecuteCommand(
                    cmd.Id,
                    m => m.UpdateNumberOfSeats(new NumberOfSeats(cmd.NumberOfSeats))),

            Meetup.V1.Publish cmd =>
                ExecuteCommand(
                    cmd.Id,
                    m => m.Publish()),

            Meetup.V1.Cancel cmd =>
                ExecuteCommand(
                    cmd.Id,
                    m => m.Cancel()),

            Meetup.V1.AcceptRSVP cmd =>
                ExecuteCommand(
                    cmd.Id,
                    m => m.AcceptRSVP(new MemberId(cmd.MemberId), DateTime.UtcNow)),

            Meetup.V1.DeclineRSVP cmd =>
                ExecuteCommand(
                    cmd.Id,
                    m => m.DeclineRSVP(new MemberId(cmd.MemberId), DateTime.UtcNow)),

            _ => throw new ApplicationException("no match")
        };

        private async Task ExecuteCommand(Guid id, Action<MeetupAggregate> command)
        {
            using var session = _documentStore.OpenSession();

            var events = await session.Events.FetchStreamAsync(id);
            var meetup = MeetupAggregate.From(events.Select(x => x.Data).ToArray());
            command(meetup);

            await ExecuteTransaction(meetup);
            session.Events.Append(id, meetup.Events);
            await session.SaveChangesAsync();
        }

        private async Task ExecuteTransaction(MeetupAggregate meetup, IDocumentSession session)
        {
            await PersistProjections(meetup, session);
            foreach (var @event in meetup.Events)
            {
                await _bus.PublishAsync((dynamic)@event);
            }
        }

        private async Task ExecuteTransaction(MeetupAggregate meetup)
        {
            using var session = _documentStore.OpenSession();
            await ExecuteTransaction(meetup, session);
        }

        private async Task PersistProjections(MeetupAggregate meetup, IDocumentSession session)
        {
            var readModel = await session.LoadAsync<AttendeesListReadModel>(meetup.Id);
            if (readModel == null)
            {
                readModel = new AttendeesListReadModel();
            }

            var updatedReadModel = new AttendeesListProjector().Project(readModel, meetup.Events.ToArray());
            session.Store(readModel);
        }

        public async Task<MeetupAggregate> Get(Guid id)
        {
            using var session = _documentStore.OpenSession();
            var events = await session.Events.FetchStreamAsync(id);
            return MeetupAggregate.From(events.Select(x => x.Data).ToArray());
        }
    }
}