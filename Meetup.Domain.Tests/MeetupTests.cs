using System;
using Xunit;
using Meetup.Domain;
using AutoFixture;
using static Meetup.Domain.Tests.MeetupTestExtensions;
using System.Collections.Generic;

namespace Meetup.Domain.Tests
{
    public class MeetupTests
    {

        [Fact]
        public void MeetupCreateTest()
        {
            var title = Auto.Create<MeetupTitle>();
            var location = Auto.Create<Location>();

            var meetup = new Meetup(title, location);
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
                    Assert.Equal(seats, meetup.NumberOfSeats);
                    Assert.Equal(MeetupState.Published, meetup.State);
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

        public static void GivenCreatedMeetup<TException>(Action<Meetup> when)
        where TException : Exception
        {
            var meetup = new Meetup(title, location);
            Assert.Throws<TException>(() => when(meetup));
        }

        public static void GivenCreatedMeetup(Action<Meetup> when, Action<Meetup> then)
        {
            var meetup = new Meetup(title, location);
            when(meetup);
            then(meetup);
        }

        public static void GivenPublishedMeetup(Action<Meetup> when, Action<Meetup> then)
        {
            var meetup = new Meetup(title, location);
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
