#nullable disable
namespace Meetup.Domain
{
    public static class Events
    {
        public class MeetupCreated
        {
            public string Title { get; set; }
            public string Location { get; set; }
        }

        public class MeetupPublished
        {
        }

        public class NumberOfSeatsUpdated
        {
            public int NumberOfSeats { get; set; }
        }
    }
}