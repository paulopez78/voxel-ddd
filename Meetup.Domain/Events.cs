#nullable disable
using System;

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

        public class MeetupCanceled
        {
        }

        public class RSVPAccepted
        {
            public Guid MemberId { get; set; }
            public DateTime AcceptedAt { get; set; }
        }

        public class RSVPDeclined
        {
            public DateTime DeclinedAt { get; set; }
            public Guid MemberId { get; set; }
        }
    }
}