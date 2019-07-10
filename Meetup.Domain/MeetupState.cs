using System;
using System.Linq;

namespace Meetup.Domain
{
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

        public override string ToString() => _name;
    }
}