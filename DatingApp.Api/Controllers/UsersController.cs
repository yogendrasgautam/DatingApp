using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using DatingApp.Api.Helpers;
using DatingApp.Api.Models;

namespace DatingApp.Api.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _datingRepo;
        private readonly IMapper _mapper;
        public UsersController(IDatingRepository datingRpo, IMapper mapper)
        {
            _datingRepo = datingRpo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _datingRepo.GetUser(currentUserId);
            userParams.UserId = user.Id;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = user.Gender == "male" ? "female" : "male";
            }
            var users = await _datingRepo.GetUsers(userParams);
            var userList = _mapper.Map<IEnumerable<UserListDto>>(users.Items);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalPages, users.TotalCount);
            return Ok(userList);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _datingRepo.GetUser(id);
            var userDetails = _mapper.Map<UserDetailsDto>(user);
            return Ok(userDetails);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto UserUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _datingRepo.GetUser(id);
            _mapper.Map(UserUpdateDto, userFromRepo);

            if (await _datingRepo.SaveAll())  
                return NoContent();

            throw new Exception($"Updating user with id {id} failled.");

        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _datingRepo.GetLike(id,recipientId);
            if(like != null)
            {
                return BadRequest("User is allready liked");
            }

            if(await _datingRepo.GetUser(recipientId) == null) 
            {
                return NotFound();
            }

            like = new Like
            {
                LikeeId = recipientId,
                LikerId = id

            };

            _datingRepo.Add<Like>(like);

            if(await _datingRepo.SaveAll())
                return Ok();

            return BadRequest("Error while liking user");
        }

    }
}