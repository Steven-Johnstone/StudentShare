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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IStudentRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IStudentRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            // compare userId against the route paramater of userId to see if they match 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized(); // deny access

            var messageFromRepo = await _repo.GetMessage(id); // gets the message from the repo

            if (messageFromRepo == null) // checking if a message is returned
                return NotFound(); // if not return not found

            return Ok(messageFromRepo); // if message found - return it
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            // compare userId against the route paramater of userId to see if they match 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized(); // deny access

            messageParams.UserId = userId; // set the message params user id to match the user id

            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams); // get the messages from the repo

            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo); // mapping the messages from repo into the DTO

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, 
            messagesFromRepo.TotalCount, messagesFromRepo.TotalPages); // adding pagination to the messages from repo results

            return Ok(messages); // return ok and pass back messages
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            var sender = await _repo.GetUser(userId);

            // compare sender against the route paramater of userId to see if they match 
            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized(); // deny access

            messageForCreationDto.SenderId = userId; // set SenderId in the repo to the userId

            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId); // get recipient from the repo

            if (recipient == null) // does the recipient exist
                return BadRequest("Could not find user"); // return error if null

            var message = _mapper.Map<Message>(messageForCreationDto); // map our messagefromcreationdto into our message model

            _repo.Add(message); // add message to the repo

            if(await _repo.SaveAll()) { // save message
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message); // maps the message into our dto
                return CreatedAtRoute("GetMessage", new{id = message.Id}, messageToReturn); // if successful return the message id and the message via the route
            } 

            throw new Exception("Creating message failed on save"); // if save fails return an error
        }

        [HttpGet("thread/{recipientId}")] // needs thread to let ASP.NET core distinguish between the previous get for just an id and this 
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            // compare userId against the route paramater of userId to see if they match 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized(); // deny access

            var messagesFromRepo = await _repo.GetMessageThread(userId, recipientId); // returning the message from the repo

            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo); // mapping the data from the repo

            return Ok(messageThread); // returning our filtered message
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            // compare userId against the route paramater of userId to see if they match 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized(); // deny access

            var messageFromRepo = await _repo.GetMessage(id); // get the message from the repo

            if (messageFromRepo.SenderId == userId) // if the senderId matches the user id, set sender deleted to true
                messageFromRepo.SenderDeleted = true;

            if (messageFromRepo.RecipientId == userId) // if the recipientId matches the recipient id, set recipient deleted to true
                messageFromRepo.RecipientDeleted = true;

            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted) // if both values are true
                _repo.Delete(messageFromRepo); // only delete message once both sides of the conversation have deleted

            if (await _repo.SaveAll())
                return NoContent(); // return nothing if save is ok

            throw new Exception("Error deleting the message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> ReadMessage(int userId, int id)
        {
            // compare userId against the route paramater of userId to see if they match 
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized(); // deny access

            var message = await _repo.GetMessage(id); // get message from the repo

            if (message.RecipientId != userId) // if recipientid doesnt match the userid, dont authorise
                return Unauthorized();

            message.IsRead = true; // set message to read
            message.DateRead = DateTime.Now; // set time of message being read

            await _repo.SaveAll();

            return NoContent();
        }

    }
}