using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Meetup.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeetupController : ControllerBase
    {
        private readonly MeetupAppService _appService;

        public MeetupController(MeetupAppService appService)
        {
            _appService = appService;
        }

        [HttpGet("{id}")]
        public Task<ActionResult> Get(Guid id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public Task<ActionResult> Post(Meetup.V1.Create request)
        {
            throw new NotImplementedException();
        }

        [HttpPut("seats")]
        public Task<ActionResult> Put(Meetup.V1.UpdateNumberOfSeats request)
        {
            throw new NotImplementedException();
        }

        [HttpPut("publish")]
        public Task<ActionResult> Put(Meetup.V1.Publish request)
        {
            throw new NotImplementedException();
        }

        [HttpPut("cancel")]
        public Task<ActionResult> Put(Meetup.V1.Cancel request)
        {
            throw new NotImplementedException();
        }

        [HttpPut("acceptrsvp")]
        public Task<ActionResult> Put(Meetup.V1.AcceptRSVP request)
        {
            throw new NotImplementedException();
        }

        [HttpPut("declinersvp")]
        public Task<ActionResult> Put(Meetup.V1.DeclineRSVP request)
        {
            throw new NotImplementedException();
        }
    }
}
