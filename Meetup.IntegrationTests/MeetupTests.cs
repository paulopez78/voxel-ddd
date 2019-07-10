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

            await _client.Create(meetupId, title, location);

            var meetup = await _client.Get(meetupId);

            Assert.Equal(meetupId, meetup.Id);
            Assert.Equal(location, meetup.Location);
            Assert.Equal(title, meetup.Title);
            Assert.Equal(Meetup.MeetupState.Created, meetup.State);
        }
    }
}
