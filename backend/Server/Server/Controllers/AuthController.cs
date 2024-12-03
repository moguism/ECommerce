using Microsoft.AspNetCore.Mvc;
using Server.Mappers;
using Server.Models;
using Server.Services;
using Server.DTOs;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("signup")]
        public async Task<string> RegisterUserAsync([FromBody] UserSignUpDto receivedUser)
        {
            return await _userService.RegisterUser(receivedUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginUser([FromBody] LoginDto userLogin)
        {
            User user = await _userService.GetUserByEmailAndPassword(userLogin.Email, userLogin.Password);
            if (user != null)
            {
                string stringToken = _userService.ObtainToken(user);
                return Ok(stringToken);
            }
            else
            {
                return Unauthorized();
            }

        }
    }
}