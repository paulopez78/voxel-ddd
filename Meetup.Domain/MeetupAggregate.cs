using System;
using System.Collections.Generic;
using System.Linq;

namespace Meetup.Domain
{
    public class MeetupAggregate
    {
        public MeetupId Id { get; private set; } = MeetupId.None;
        public MeetupTitle Title { get; private set; } = MeetupTitle.None;
        public Location Location { get; private set; } = Location.None;
        public NumberOfSeats NumberOfSeats { get; private set; } = NumberOfSeats.None;
        public MeetupState State { get; private set; } = MeetupState.None;
        private Dictionary<MemberId, DateTime> _membersGoing = new Dictionary<MemberId, DateTime>();
        public IReadOnlyDictionary<MemberId, DateTime> MembersGoing => _membersGoing;
        private Dictionary<MemberId, DateTime> _membersNotGoing = new Dictionary<MemberId, DateTime>();
        public IReadOnlyDictionary<MemberId, DateTime> MembersNotGoing => _membersNotGoing;
        private List<object> _events = new List<object>();
        public IEnumerable<object> Events => _events.AsEnumerable();

        public MeetupAggregate(
            MeetupId id,
            MeetupTitle title,
            Location location,
            NumberOfSeats seats,
            MeetupState state,
            Dictionary<MemberId, DateTime> going,
            Dictionary<MemberId, DateTime> notGoing)
        {
            Id = id;
            Title = title;
            Location = location;
            NumberOfSeats = seats;
            State = state;
            _membersGoing = going;
            _membersNotGoing = notGoing;
        }

        public MeetupAggregate(MeetupId id, MeetupTitle title, Location location) =>
            Apply(new Events.MeetupCreated { MeetupId = id, Title = title, Location = location });

        public void UpdateNumberOfSeats(NumberOfSeats seats) =>
            Apply(new Events.NumberOfSeatsUpdated { MeetupId = Id, NumberOfSeats = seats });

        public void Publish()
        {
            if (NumberOfSeats == NumberOfSeats.None) throw new ArgumentException(nameof(NumberOfSeats));
            State.TransitionTo(MeetupState.Published);

            Apply(new Events.MeetupPublished { MeetupId = Id });
        }

        public void Cancel()
        {
            State = State.TransitionTo(MeetupState.Canceled);
            Apply(new Events.MeetupCanceled { MeetupId = Id });
        }

        public void AcceptRSVP(MemberId memberId, DateTime acceptedAt)
        {
            if (State != MeetupState.Published)
                throw new ArgumentException(nameof(memberId));

            Apply(new Events.RSVPAccepted { MeetupId = Id, MemberId = memberId, AcceptedAt = acceptedAt });
        }

        public void DeclineRSVP(MemberId memberId, DateTime declineAt)
        {
            if (State != MeetupState.Published)
                throw new ArgumentException(nameof(memberId));

            Apply(new Events.RSVPDeclined { MeetupId = Id, MemberId = memberId, DeclinedAt = declineAt });
        }

        public void Apply(object @event)
        {
            _events.Add(@event);
            When(@event);
        }

        public void When(object @event)
        {
            switch (@event)
            {
                case Events.MeetupCreated created:
                    Title = MeetupTitle.From(created.Title);
                    Location = Location.From(created.Location);
                    State = MeetupState.Created;
                    NumberOfSeats = NumberOfSeats.None;
                    _membersGoing = new Dictionary<MemberId, DateTime>();
                    _membersNotGoing = new Dictionary<MemberId, DateTime>();
                    break;

                case Events.NumberOfSeatsUpdated seatsUpdated:
                    NumberOfSeats = NumberOfSeats.From(seatsUpdated.NumberOfSeats);
                    break;

                case Events.MeetupPublished published:
                    State = MeetupState.Published;
                    break;

                case Events.MeetupCanceled canceled:
                    State = MeetupState.Canceled;
                    break;

                case Events.RSVPAccepted accepted:
                    _membersGoing.Add(MemberId.From(accepted.MemberId), accepted.AcceptedAt);
                    break;

                case Events.RSVPDeclined declined:
                    _membersNotGoing.Add(MemberId.From(declined.MemberId), declined.DeclinedAt);
                    break;
            }
        }
    }
}