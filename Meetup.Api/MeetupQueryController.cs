using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Meetup.Api.Controllers
{
    [Route("api/meetup/attendants")]
    [ApiController]
    public class MeetupQueryController : ControllerBase
    {
        private readonly AttendeesRepository _repo;

        public MeetupQueryController(AttendeesRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            var attendes = await _repo.Get(id);
            return Ok(attendes);
        }
    }
}
