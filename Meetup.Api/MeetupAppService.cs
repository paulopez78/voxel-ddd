using System;
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

        public MeetupAppService(MeetupRepository repo, AddressValidator addressValidator, IBus bus)
        {
            _repo = repo;
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

        private Task ExecuteTransaction(MeetupAggregate meetup) => _repo.Save(meetup);

        private async Task ExecuteCommand(Guid id, Action<MeetupAggregate> command)
        {
            var meetup = await _repo.Get(id);
            command(meetup);
            await ExecuteTransaction(meetup);

            foreach (var ev in meetup.Events)
            {
                await _bus.PublishAsync((dynamic)ev);
            }
        }

        public Task<MeetupAggregate> Get(Guid id) => _repo.Get(id);
    }
}