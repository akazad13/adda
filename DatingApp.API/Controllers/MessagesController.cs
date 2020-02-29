using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTOs;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/users/{userId}/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMapper _mapper;
        public IDatingRepository _repo { get; set; }
        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;

        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo == null)
            {
                return NoContent();
            }

            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            messageParams.UserId = userId;

            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);
            var messages = _mapper.Map<IEnumerable<MessageToReturnDTO>>(messagesFromRepo);
            Response.AddPagination(messagesFromRepo.CurrrentPage, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);
            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messagesFromRepo = await _repo.GetMessageThread(userId, recipientId);
            var messages = _mapper.Map<IEnumerable<MessageToReturnDTO>>(messagesFromRepo);

            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDTO messageForCreationDTO)
        {
            var sender = await _repo.GetUser(userId, true);

            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            messageForCreationDTO.SenderId = userId;
            var recipient = await _repo.GetUser(messageForCreationDTO.RecipientId, false);

            if (recipient == null)
            {
                return BadRequest("Could not find user");
            }

            var message = _mapper.Map<Message>(messageForCreationDTO);
            _repo.Add(message);

            if (await _repo.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageToReturnDTO>(message);
                return CreatedAtRoute("GetMessage", new { userId, id = message.Id }, messageToReturn);
            }

            return BadRequest("Creating the message failed on save");

        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int userId, int id)
        {

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo == null)
            {
                return BadRequest("Could not find user");
            }

            if (messageFromRepo.SenderId == userId)
            {
                messageFromRepo.SenderDeleted = true;
            }
            if (messageFromRepo.RecipientId == userId)
            {
                messageFromRepo.RecipientDeleted = true;
            }

            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
            {
                _repo.Delete(messageFromRepo);
            }

            if (await _repo.SaveAll())
            {
                return NoContent();
            }

            throw new Exception("Error deleting the message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var message = await _repo.GetMessage(id);

            if (message == null)
            {
                return BadRequest("Could not find user");
            }

            if (message.RecipientId != userId)
            {
                return Unauthorized();
            }

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            if (await _repo.SaveAll())
            {
                return NoContent();
            }

            throw new Exception("Error marking the message as read");
        }
    }
}