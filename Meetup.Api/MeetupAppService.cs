using System;
using System.Threading.Tasks;
using Meetup.Domain;

namespace Meetup.Api
{
    public class MeetupAppService
    {
        private readonly MeetupRepository _repo;
        private readonly AddressValidator _addressValidator;

        public MeetupAppService(MeetupRepository repo, AddressValidator addressValidator)
        {
            _repo = repo;
            _addressValidator = addressValidator;
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

            _ => throw new ApplicationException("no match")
        };

        private async Task ExecuteTransaction(MeetupAggregate meetup)
        {
            await _repo.Save(meetup);
        }

        private async Task ExecuteCommand(Guid id, Action<MeetupAggregate> command)
        {
            var meetup = await _repo.Get(id);
            command(meetup);
            await ExecuteTransaction(meetup);
        }

        public Task<MeetupAggregate> Get(Guid id) => _repo.Get(id);
    }
}