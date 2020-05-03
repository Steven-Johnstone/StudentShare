using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentShare.API.Data;

namespace StudentShare.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context; // good practice to put _ if private
        public ValuesController(DataContext context) // inject the database into the constructor
        {
            _context = context; 

        }
        [AllowAnonymous]
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> GetValues() // Async to allow more than one user to access at a time
        {
            var values = await _context.Values.ToListAsync(); //goes and gets values from the database as a list

            return Ok(values); // returns the values
        }

        [AllowAnonymous]
        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id) // Async to allow more than one user to access at a time
        {  //goes and gets only the requested value and stores it in value
            var value = await _context.Values.FirstOrDefaultAsync(x => x.Id == id); 

            return Ok(value);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
