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
using StudentShare.API.Models;

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
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value); // sets currentuser to matching user

            var userFromRepo = await _repo.GetUser(currentUserId); // gets the user from the repo

            userParams.UserId = currentUserId; // stores the current user in user params

            var users = await _repo.GetUsers(userParams); // stores the user via getusers method into users

            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users); // maps the info from user into our ListDto

            Response.AddPagination(users.CurrentPage, users.PageSize, 
            users.TotalCount, users.TotalPages); // used to return this info back via the header to the client

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

            if (await _repo.SaveAll()) // upon saving the update, if OK, return nothing
            return NoContent();

            throw new Exception($"Updating user {id} failed."); // if it fails, return bad message
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {   // compare userId against the route paramater of userId to see if they match 
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized(); // deny access

            var like = await _repo.GetLike(id, recipientId); // get like from repo

            if (like != null) // if the like exists - return bad request message
                return BadRequest("You already like this user");

            if (await _repo.GetUser(recipientId) == null) //query the repo again to check for recipientId
                return NotFound(); // if not found - return notfound

            like = new Like { // create new like with the likerid and likeeid
                LikerId = id,
                LikeeId = recipientId
            };

            _repo.Add<Like>(like); // saves the like into memory

            if (await _repo.SaveAll()) // upon saving the like, if OK, return ok
                return Ok();
            
            return BadRequest("Failed to like user"); // if it fails, return bad message
        }

    }
}