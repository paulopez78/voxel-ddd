using System;

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
            State = MeetupState.Published;
        }
    }

    public enum MeetupState
    {
        Created,
        Published
    }
}