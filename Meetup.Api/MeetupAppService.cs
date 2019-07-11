using System;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using Meetup.Domain;

namespace Meetup.Api
{
    public class MeetupAppService
    {
        private readonly MeetupRepository _repo;
        private readonly AddressValidator _addressValidator;
        private readonly IBus _bus;
        private readonly AttendeesRepository _readModelRepo;

        public MeetupAppService(MeetupRepository repo, AttendeesRepository readModelRepo, AddressValidator addressValidator, IBus bus)
        {
            _repo = repo;
            _addressValidator = addressValidator;
            _bus = bus;
            _readModelRepo = readModelRepo;
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
            var meetup = await Get(id);
            command(meetup);
            await ExecuteTransaction(meetup);
        }

        private async Task ExecuteTransaction(MeetupAggregate meetup)
        {
            await _repo.Save(meetup);
            await PersistProjections(meetup);

            foreach (var @event in meetup.Events)
            {
                await _bus.PublishAsync((dynamic)@event);
            }
        }

        private async Task PersistProjections(MeetupAggregate meetup)
        {
            var readModel = await _readModelRepo.Get(meetup.Id);
            if (readModel == null)
            {
                readModel = new AttendeesListReadModel();
            }

            var updatedReadModel = new AttendeesListProjector().Project(readModel, meetup.Events.ToArray());
            await _readModelRepo.Save(updatedReadModel);
        }

        public Task<MeetupAggregate> Get(Guid id) => _repo.Get(id);
    }
}