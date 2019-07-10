using System;
using Xunit;
using Meetup.Domain;
using AutoFixture;

namespace Meetup.Domain.Tests
{
    public class MeetupTests
    {
        public Fixture Auto { get; } = new Fixture();

        [Fact]
        public void MeetupCreateTest()
        {
            var title = Auto.Create<string>();
            var location = Auto.Create<string>();

            var meetup = new Meetup(title, location);
            Assert.Equal(title, meetup.Title);
            Assert.Equal(location, meetup.Location);
            Assert.Equal(MeetupState.Created, meetup.State);
        }

        [Fact]
        public void MeetupPublishTest()
        {
            var title = Auto.Create<string>();
            var location = Auto.Create<string>();
            var seats = Auto.Create<int>();

            var meetup = new Meetup(title, location);
            meetup.UpdateNumberOfSeats(seats);
            meetup.Publish();

            Assert.Equal(seats, meetup.NumberOfSeats);
            Assert.Equal(MeetupState.Published, meetup.State);
        }

        [Fact]
        public void MeetupInvalidNumberOfSeatsTest()
        {
            var title = Auto.Create<string>();
            var location = Auto.Create<string>();
            var seats = 0;

            var meetup = new Meetup(title, location);
            Assert.Throws<ArgumentException>(
                () => meetup.Publish());
            Assert.Throws<ArgumentException>(
                () => meetup.UpdateNumberOfSeats(seats));
        }
    }
}
