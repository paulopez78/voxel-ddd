using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Meetup.Domain;
using Xunit;
using static Meetup.Domain.Tests.AttendeesListTestsExtensions;

namespace Meetup.Domain.Tests
{
    public class AttendeesListTest
    {
        [Fact]
        public void Test1() =>
            GivenPublishedMeetup(
                withCapacity: 2,
                when: (p, r) => p.Project(r,
                    RSVPAccepted(bob),
                    RSVPAccepted(carla),
                    RSVPAccepted(bill))
                ,
                then: r =>
                {
                    r.MembersGoing.AssertEqual(bob, carla);
                    r.WaitingList.AssertEqual(bill);
                    Assert.Empty(r.MembersNotGoing);
                }
            );

        [Fact]
        public void Test2()
        {
            var fixture = new Fixture();
            var id = fixture.Create<Guid>();
            var title = fixture.Create<string>();
            var location = fixture.Create<string>();
            var seats = 2;

            var bob = fixture.Create<Guid>();
            var bill = fixture.Create<Guid>();
            var carla = fixture.Create<Guid>();

            var projector = new AttendeesListProjector();

            var readModelUpdated = projector.Project(new AttendeesListReadModel(),
                new Events.MeetupCreated { MeetupId = id, Title = title, Location = location },
                new Events.NumberOfSeatsUpdated { MeetupId = id, NumberOfSeats = seats },
                new Events.MeetupPublished { MeetupId = id, },
                new Events.RSVPAccepted { MeetupId = id, MemberId = bob },
                new Events.RSVPAccepted { MeetupId = id, MemberId = carla },
                new Events.RSVPAccepted { MeetupId = id, MemberId = bill },
                new Events.RSVPDeclined { MeetupId = id, MemberId = bob }
                 );

            readModelUpdated.MembersGoing.AssertEqual(carla, bill);
            readModelUpdated.MembersNotGoing.AssertEqual(bob);
            Assert.Empty(readModelUpdated.WaitingList);
        }
    }
    public static class AttendeesListTestsExtensions
    {
        public static Fixture fixture = new Fixture();
        public static Guid id = fixture.Create<Guid>();
        public static string title = fixture.Create<string>();
        public static string location = fixture.Create<string>();
        public static Guid bob = fixture.Create<Guid>();
        public static Guid bill = fixture.Create<Guid>();
        public static Guid carla = fixture.Create<Guid>();
        public static void GivenPublishedMeetup(int withCapacity, Action<AttendeesListProjector, AttendeesListReadModel> when, Action<AttendeesListReadModel> then)
        {

            var projector = new AttendeesListProjector();
            var readModelUpdated = projector.Project(new AttendeesListReadModel(),
                new Events.MeetupCreated { MeetupId = id, Title = title, Location = location },
                new Events.NumberOfSeatsUpdated { MeetupId = id, NumberOfSeats = withCapacity },
                new Events.MeetupPublished { MeetupId = id, });

            when(projector, readModelUpdated);
            then(readModelUpdated);
        }

        public static Events.RSVPAccepted RSVPAccepted(Guid memberId) =>
            new Events.RSVPAccepted { MemberId = memberId };
        public static Events.RSVPDeclined RSVPDeclined(Guid memberId) =>
            new Events.RSVPDeclined { MemberId = memberId };

        public static void AssertEqual(this List<Guid> @this, params Guid[] members)
        {
            Assert.Equal(members.ToList(), @this);
        }
    }
}