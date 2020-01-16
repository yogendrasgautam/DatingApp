using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.Api.Data;
using DatingApp.Api.DTOs;
using DatingApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthRepository _authRepo;
        private IConfiguration _config;
        private  IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _authRepo = repo;
            _config = config;
            _mapper = mapper;
        }

        
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto userRegistration)
        {
            userRegistration.Username = userRegistration.Username.ToLower();

            if(await _authRepo.UserExists(userRegistration.Username))
                return BadRequest("User allready exist");

            var userToCreate = _mapper.Map<User>(userRegistration);

            var userCreated = await _authRepo.Register(userToCreate, userRegistration.Password);
            var userToReturn = _mapper.Map<UserDetailsDto>(userCreated);

            return CreatedAtRoute("GetUser", new {controller= "Users", id= userCreated.Id }, userToReturn);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLogin)
        {
          
            var loggedInUser = await _authRepo.Login(userLogin.Username.ToLower(),userLogin.Password);
            if(loggedInUser == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, loggedInUser.Id.ToString()),
                new Claim(ClaimTypes.Name, loggedInUser.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value.ToString()));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = cred
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var user = _mapper.Map<UserListDto>(loggedInUser);

            return Ok(new {
                token = tokenHandler.WriteToken(token),
                user
            });


        }
    }
}