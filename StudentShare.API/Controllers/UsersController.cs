using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentShare.API.Data;
using StudentShare.API.Dtos;
using StudentShare.API.Helpers;

namespace StudentShare.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))] //updates the last active date based on any of these methods being called
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

        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto userUpdateDto)
        {   // compare userId against the route paramater of userId to see if they match 
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized(); // deny access

            var userFromRepo = await _repo.GetUser(id); // gets the userId from the database

            _mapper.Map(userUpdateDto, userFromRepo);

            if (await _repo.SaveAll())
            return NoContent();

            throw new Exception($"Updating user {id} failed.");
        }

    }
}