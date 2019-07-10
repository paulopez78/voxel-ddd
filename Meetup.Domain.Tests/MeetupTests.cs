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
            var title = Auto.Create<MeetupTitle>();
            var location = Auto.Create<Location>();

            var meetup = new Meetup(title, location);
            Assert.Equal(title, meetup.Title);
            Assert.Equal(location, meetup.Location);
            Assert.Equal(MeetupState.Created, meetup.State);
        }

        [Fact]
        public void MeetupPublishTest()
        {
            var title = Auto.Create<MeetupTitle>();
            var location = Auto.Create<Location>();
            var seats = new NumberOfSeats(10);

            var meetup = new Meetup(title, location);
            meetup.UpdateNumberOfSeats(seats);
            meetup.Publish();

            Assert.Equal(seats, meetup.NumberOfSeats);
            Assert.Equal(MeetupState.Published, meetup.State);
        }

        [Fact]
        public void MeetupInvalidNumberOfSeatsTest()
        {
            var title = Auto.Create<MeetupTitle>();
            var location = Auto.Create<Location>();

            var meetup = new Meetup(title, location);
            Assert.Throws<ArgumentException>(
                () => meetup.Publish());
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
}
