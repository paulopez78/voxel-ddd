
using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Meetup.IntegrationTests
{
    public class MeetupClientFixture : IDisposable
    {
        public MeetupClient MeetupClient { get; }

        public MeetupClientFixture()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var servicesCollection = new ServiceCollection();

            servicesCollection.AddSingleton<IConfiguration>(configuration);
            servicesCollection.AddHttpClient<MeetupClient>();

            var serviceProvider = servicesCollection.BuildServiceProvider();
            MeetupClient = serviceProvider.GetService<MeetupClient>();
        }

        public void Dispose()
        {

        }
    }
}