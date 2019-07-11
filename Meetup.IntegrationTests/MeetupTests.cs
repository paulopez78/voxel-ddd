using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Xunit;

namespace Meetup.IntegrationTests
{
    public class MeetupTests : IClassFixture<MeetupClientFixture>
    {
        private readonly MeetupClient _client;

        public MeetupTests(MeetupClientFixture fixture) => _client = fixture.MeetupClient;

        [Fact]
        public async Task Meetup_Create_Test()
        {
            var auto = new Fixture();
            var meetupId = auto.Create<Guid>();
            var title = auto.Create<string>();
            var location = auto.Create<string>();
            var seats = auto.Create<int>();
            var bob = auto.Create<Guid>();
            var carla = auto.Create<Guid>();

            await _client.Create(meetupId, title, location);
            await _client.UpdateSeats(meetupId, seats);
            await _client.Publish(meetupId);
            await _client.AcceptRSVP(meetupId, bob, DateTime.UtcNow);
            await _client.RejectRSVP(meetupId, carla, DateTime.UtcNow);

            var meetup = await _client.Get(meetupId);

            Assert.Equal(meetupId, meetup.Id);
            Assert.Equal(location, meetup.Location);
            Assert.Equal(title, meetup.Title);
            Assert.Equal(seats, meetup.NumberOfSeats);
            Assert.Equal(Meetup.MeetupState.Published, meetup.State);
            Assert.True(meetup.MembersGoing.ContainsKey(bob));
            Assert.True(meetup.MembersNotGoing.ContainsKey(carla));
        }
    }
}
