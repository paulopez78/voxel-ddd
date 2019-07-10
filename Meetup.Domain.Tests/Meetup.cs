using System;

namespace Meetup.Domain
{
    public class Meetup
    {
        public string Title { get; }
        public string Location { get; }
        public int NumberOfSeats { get; private set; }
        public MeetupState State { get; private set; }

        public Meetup(string title, string location)
        {
            Title = title;
            Location = location;
            State = MeetupState.Created;
        }

        public void UpdateNumberOfSeats(int seats)
        {
            if (seats < 1 || seats > 1000) throw new ArgumentException(nameof(seats));
            NumberOfSeats = seats;
        }

        public void Publish()
        {
            if (NumberOfSeats < 1 || NumberOfSeats > 1000) throw new ArgumentException(nameof(NumberOfSeats));
            State = MeetupState.Published;
        }
    }

    public enum MeetupState
    {
        Created,
        Published
    }
}