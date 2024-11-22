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

        private readonly UserMapper _userMapper;
        private readonly PasswordService _passwordService;
        private readonly UserService _userService;

        public AuthController( UserMapper userMapper, PasswordService passwordService, UserService userService)
        {
            _userMapper = userMapper;
            _passwordService = passwordService;
            _userService = userService;
        }

        [HttpPost("signup")]
        public async Task<string> RegisterUserAsync([FromBody] UserSignUpDto receivedUser)
        {
            User user = _userMapper.ToEntity(receivedUser);
            user.Password = _passwordService.Hash(receivedUser.Password);
            await _userService.InsertUser(user);
            return _userService.ObtainToken(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginUser([FromBody] LoginDto userLogin)
        {
            User user = await _userService.GetUserByEmailAsync(userLogin.Email);
            if (user != null && _passwordService.IsPasswordCorrect(user.Password, userLogin.Password))
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