using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Meetup.IntegrationTests
{
    public class MeetupClient
    {
        private HttpClient _client;

        public MeetupClient(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            client.BaseAddress = new Uri(configuration["meetupUrl"] ?? "http://localhost:5000/api/meetup/");
        }

        public Task<HttpResponseMessage> Create(Guid id, string title, string location) =>
            _client.PostAsJsonAsync("", new
            {
                Id = id,
                Title = title,
                Location = location
            });


        public Task<HttpResponseMessage> UpdateSeats(Guid id, int seats) =>
            _client.PutAsJsonAsync("seats", new
            {
                Id = id,
                NumberOfSeats = seats
            });

        public Task<HttpResponseMessage> Publish(Guid id) =>
            _client.PutAsJsonAsync("publish", new
            {
                Id = id,
            });

        public Task<HttpResponseMessage> AcceptRSVP(Guid id, Guid memberId, DateTime acceptedAt) =>
            _client.PutAsJsonAsync("acceptrsvp", new
            {
                Id = id,
                MemberId = memberId,
                AcceptedAt = acceptedAt
            });

        public Task<HttpResponseMessage> RejectRSVP(Guid id, Guid memberId, DateTime rejectedAt) =>
            _client.PutAsJsonAsync("declinersvp", new
            {
                Id = id,
                MemberId = memberId,
                RejectedAt = rejectedAt
            });

        public Task<HttpResponseMessage> Cancel(Guid id) =>
            _client.PutAsJsonAsync("cancel", new
            {
                Id = id,
            });

        public async Task<Meetup> Get(Guid id)
        {
            var response = await _client.GetAsync($"{id}");
            return await response.Content.ReadAsAsync<Meetup>();
        }
    }

    public class Meetup
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public int NumberOfSeats { get; set; }
        public MeetupState State { get; set; }
        public Dictionary<Guid, DateTime> MembersGoing { get; set; }
        public Dictionary<Guid, DateTime> MembersNotGoing { get; set; }

        public enum MeetupState
        {
            Created,
            Published,
            Canceled,
        }
    }
}