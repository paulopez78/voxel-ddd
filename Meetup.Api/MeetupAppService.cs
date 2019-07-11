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

        public async Task Handle(object command)
        {
            switch (command)
            {
                case Meetup.V1.Create cmd:
                    await ExecuteTransaction(
                    new MeetupAggregate(
                        new MeetupId(cmd.Id),
                        new MeetupTitle(cmd.Title),
                        new Location(_addressValidator, cmd.Location)));
                    break;

                case Meetup.V1.UpdateNumberOfSeats cmd:
                    await ExecuteCommand(
                        cmd.Id,
                        m => m.UpdateNumberOfSeats(new NumberOfSeats(cmd.NumberOfSeats)));
                    break;

                case Meetup.V1.Publish cmd:
                    await ExecuteCommand(
                        cmd.Id,
                        m => m.Publish());
                    break;

                default:
                    throw new ApplicationException("no match");
            }
        }

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