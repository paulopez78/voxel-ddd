using System;
using Xunit;
using Meetup.Domain;
using AutoFixture;
using static Meetup.Domain.Tests.MeetupTestExtensions;
using System.Collections.Generic;
using System.Linq;

namespace Meetup.Domain.Tests
{
    public class MeetupTests
    {

        [Fact]
        public void MeetupCreateTest()
        {
            var title = Auto.Create<MeetupTitle>();
            var location = Auto.Create<Location>();

            var meetup = new MeetupAggregate(title, location);
            Assert.Equal(title, meetup.Title);
            Assert.Equal(location, meetup.Location);
            Assert.Equal(MeetupState.Created, meetup.State);
        }

        [Fact]
        public void GivenCreatedMeetup_When_Publish_WithNumberOfSeats_Then_Published() =>
            GivenCreatedMeetup(
                when: meetup =>
                {
                    meetup.UpdateNumberOfSeats(seats);
                    meetup.Publish();
                },
                then: meetup =>
                {
                    Assert.IsType<Events.MeetupCreated>(meetup.Events.First());
                    Assert.IsType<Events.MeetupPublished>(meetup.Events.Last());

                    // Assert.Equal(seats, meetup.NumberOfSeats);
                    // Assert.Equal(MeetupState.Published, meetup.State);
                }
            );

        [Fact]
        public void GivenCreatedMeetup_When_Publish_Then_Throws() =>
            GivenCreatedMeetup<ArgumentException>(
                when: meetup => meetup.Publish()
            );

        [Fact]
        public void GivenCreatedMeetup_When_Cancel_Then_Throws() =>
            GivenCreatedMeetup<ArgumentException>(
                when: meetup => meetup.Cancel()
            );

        [Fact]
        public void GivenPublishedMeetup_When_AcceptRSVP_Then_MemberGoing()
        {
            var memberId = Auto.Create<MemberId>();
            var acceptedAt = Auto.Create<DateTime>();

            GivenPublishedMeetup(
                when: meetup => meetup.AcceptRSVP(memberId, acceptedAt),
                then: meetup => meetup.MembersGoing.AssertContains(memberId)
            );
        }

        [Fact]
        public void GivenPublishedMeetup_When_DeclineRSVP_Then_MemberNotGoing()
        {
            var memberId = Auto.Create<MemberId>();
            var declinedAt = Auto.Create<DateTime>();

            GivenPublishedMeetup(
                when: meetup => meetup.DeclineRSVP(memberId, declinedAt),
                then: meetup => meetup.MembersNotGoing.AssertContains(memberId)
            );
        }

        [Theory]
        [InlineData(1)]
        [InlineData(1000)]
        public void ValidNumberOfSeatsTest(int seats)
        {
            Assert.Equal(seats, new NumberOfSeats(seats));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(1001)]
        public void InvalidNumberOfSeatsTest(int seats)
        {
            Assert.Throws<ArgumentException>(
                () => new NumberOfSeats(seats));
        }

        [Fact]
        public void EqualNumberOfSeats()
        {
            var a = new NumberOfSeats(10);
            var b = new NumberOfSeats(10);

            Assert.Equal(a, b);
            Assert.True(a == b);
        }
    }

    public static class MeetupTestExtensions
    {
        public static Fixture Auto { get; } = new Fixture();
        public static MeetupTitle title = Auto.Create<MeetupTitle>();
        public static Location location = Auto.Create<Location>();
        public static NumberOfSeats seats = new NumberOfSeats(10);

        public static void GivenCreatedMeetup<TException>(Action<MeetupAggregate> when)
        where TException : Exception
        {
            var meetup = new MeetupAggregate(title, location);
            Assert.Throws<TException>(() => when(meetup));
        }

        public static void GivenCreatedMeetup(Action<MeetupAggregate> when, Action<MeetupAggregate> then)
        {
            var meetup = new MeetupAggregate(title, location);
            when(meetup);
            then(meetup);
        }

        public static void GivenPublishedMeetup(Action<MeetupAggregate> when, Action<MeetupAggregate> then)
        {
            var meetup = new MeetupAggregate(title, location);
            meetup.UpdateNumberOfSeats(seats);
            meetup.Publish();
            when(meetup);
            then(meetup);
        }

        public static void AssertContains(this IReadOnlyDictionary<MemberId, DateTime> @this, MemberId memberId)
        {
            Assert.True(@this.ContainsKey(memberId));
        }
    }
}
