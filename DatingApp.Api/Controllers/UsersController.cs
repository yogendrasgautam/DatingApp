using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace DatingApp.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly IDatingRepository _datingRepo;
        private readonly IMapper _mapper; 
        public UsersController(IDatingRepository datingRpo, IMapper mapper)
        {
            _datingRepo = datingRpo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _datingRepo.GetUsers();
            var userList = _mapper.Map<IEnumerable<UserListDto>>(users);
            return Ok(userList);
        }

        [HttpGet("{id}", Name="GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _datingRepo.GetUser(id);
            var userDetails = _mapper.Map<UserDetailsDto>(user);
            return Ok(userDetails);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto UserUpdateDto){
            if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _datingRepo.GetUser(id);
            _mapper.Map(UserUpdateDto,userFromRepo );
            
            if(await _datingRepo.SaveAll())
                return NoContent();

            throw new Exception($"Updating user with id {id} failled.");

        }

   }
}