using System;
using System.Collections.Generic;
using System.Linq;

namespace Meetup.Domain
{
    public class Meetup
    {
        public MeetupTitle Title { get; }
        public Location Location { get; }
        public NumberOfSeats NumberOfSeats { get; private set; }
        public MeetupState State { get; private set; }

        public Meetup(MeetupTitle title, Location location)
        {
            Title = title;
            Location = location;
            State = MeetupState.Created;
            NumberOfSeats = NumberOfSeats.None;
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