using System;
using EasyNetQ;
using Marten;
using Meetup.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Meetup.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton<AddressValidator>(address => true);
            services.AddScoped<MeetupAppService>();
            services.AddScoped<MeetupRepository>();
            services.AddScoped<AttendeesRepository>();
            var messageBroker = Configuration.GetValue("messagebroker", "host=localhost;username=guest;password=guest;publisherConfirms=true");
            services.AddSingleton<IBus>(RabbitHutch.CreateBus(messageBroker));

            AddEventStore();

            void AddEventStore() =>
                Retry(() => services.AddSingleton<IDocumentStore>(DocumentStore.For(_ =>
                {
                    _.Connection(Configuration["eventstore"] ?? "Host=localhost;Port=5433;Username=postgres;Password=changeit");
                    _.Events.DatabaseSchemaName = "meetup";
                    _.DatabaseSchemaName = "meetup";
                })));

            void Retry(Action action, int retries = 3) =>
                Policy.Handle<Exception>()
                    .WaitAndRetry(retries, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                    .Execute(action);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
