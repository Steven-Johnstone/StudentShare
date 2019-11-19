using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentShare.API.Data;
using StudentShare.API.Dtos;

namespace StudentShare.API.Controllers
{
    [Authorize] // users need to be authorised before access is granted to info
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IStudentRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IStudentRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users); // maps the info from user into our ListDto

            return Ok(usersToReturn); // returns the details from the DTO 
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailsDto>(user); // maps the info from user into our DetailsDto

            return Ok(userToReturn); // returns the details from the DTO 
        }

    }
}