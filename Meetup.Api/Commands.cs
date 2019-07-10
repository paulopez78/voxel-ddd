using System;

namespace Meetup.Api
{
    public static class Meetup
    {
        public static class V1
        {
            public class Create
            {
                public Guid Id { get; set; }
                public string Title { get; set; }
                public string Location { get; set; }
            }

            public class UpdateNumberOfSeats
            {
                public Guid Id { get; set; }
                public int NumberOfSeats { get; set; }
            }

            public class Publish
            {
                public Guid Id { get; set; }
            }

            public class Cancel
            {
                public Guid Id { get; set; }
            }

            public class AcceptRSVP
            {
                public Guid Id { get; set; }
                public Guid MemberId { get; set; }
            }

            public class DeclineRSVP
            {
                public Guid Id { get; set; }
                public Guid MemberId { get; set; }
            }
        }
    }
}