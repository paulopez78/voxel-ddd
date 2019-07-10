using Meetup.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            // var messageBroker = Configuration.GetValue("messagebroker", "host=localhost;username=guest;password=guest;publisherConfirms=true");
            // services.AddSingleton<IBus>(RabbitHutch.CreateBus(messageBroker));
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
