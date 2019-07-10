using System;
using System.Collections.Generic;
using System.Linq;

namespace Meetup.Domain
{
    public class MeetupAggregate
    {
        public MeetupTitle Title { get; private set; } = MeetupTitle.None;
        public Location Location { get; private set; } = Location.None;
        public NumberOfSeats NumberOfSeats { get; private set; } = NumberOfSeats.None;
        public MeetupState State { get; private set; } = MeetupState.None;
        private Dictionary<MemberId, DateTime> _membersGoing = new Dictionary<MemberId, DateTime>();
        public IReadOnlyDictionary<MemberId, DateTime> MembersGoing => _membersGoing;
        private Dictionary<MemberId, DateTime> _membersNotGoing = new Dictionary<MemberId, DateTime>();

        public IReadOnlyDictionary<MemberId, DateTime> MembersNotGoing => _membersNotGoing;

        public MeetupAggregate(MeetupTitle title, Location location)
        {
            When(new Events.MeetupCreated { Title = title, Location = location });
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
            }
        }

        public void UpdateNumberOfSeats(NumberOfSeats seats)
        {
            NumberOfSeats = seats;
        }

        public void Publish()
        {
            if (NumberOfSeats == NumberOfSeats.None) throw new ArgumentException(nameof(NumberOfSeats));
            State = State.TransitionTo(MeetupState.Published);
        }

        public void Cancel()
        {
            State = State.TransitionTo(MeetupState.Canceled);
        }

        public void AcceptRSVP(MemberId memberId, DateTime acceptedAt)
        {
            if (State != MeetupState.Published)
                throw new ArgumentException(nameof(memberId));

            _membersGoing.Add(memberId, acceptedAt);
        }

        public void DeclineRSVP(MemberId memberId, DateTime declineAt)
        {
            if (State != MeetupState.Published)
                throw new ArgumentException(nameof(memberId));

            _membersNotGoing.Add(memberId, declineAt);
        }
    }

    public class MeetupState
    {
        public static readonly MeetupState Canceled = new MeetupState(nameof(Canceled));
        public static readonly MeetupState Published = new MeetupState(nameof(Published), Canceled);
        public static readonly MeetupState Created = new MeetupState(nameof(Created), Published);
        public static readonly MeetupState None = new MeetupState(nameof(None), Created);
        private readonly string _name;
        private readonly MeetupState[] _allowedTransitions;

        private MeetupState(string name, params MeetupState[] allowedTransitions)
        {
            _name = name;
            _allowedTransitions = allowedTransitions;
        }

        public bool CanTransitionTo(MeetupState to) =>
            _allowedTransitions.Any(x => x == to);

        public MeetupState TransitionTo(MeetupState to)
        {
            if (!CanTransitionTo(to))
                throw new ArgumentException($"{nameof(to)}");

            return to;
        }
    }
}