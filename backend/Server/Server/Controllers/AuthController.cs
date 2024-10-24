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
        private readonly UnitOfWork _unitOfWork;
        private readonly PasswordService _passwordService;
        private readonly TokenValidationParameters _tokenParameters;

        public AuthController(UnitOfWork unitOfWork, UserMapper userMapper, PasswordService passwordService, IOptionsMonitor<JwtBearerOptions> jwtOptions)
        {
            _unitOfWork = unitOfWork;
            _userMapper = userMapper;
            _passwordService = passwordService;
            _tokenParameters = jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme)
                .TokenValidationParameters;
        }

        [HttpPost("signup")]
        public async Task RegisterUserAsync([FromBody] UserSignUpDto receivedUser)
        {           
            User user = _userMapper.ToEntity(receivedUser);
            user.Password = _passwordService.Hash(receivedUser.Password);
            await _unitOfWork.UserRepository.InsertAsync(user);
            await _unitOfWork.SaveAsync();
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> LoginUser([FromBody] LoginDto userLogin)
        {
            User user = await _unitOfWork.UserRepository.GetByEmailAsync(userLogin.Email);
            if(user != null && _passwordService.IsPasswordCorrect(user.Password, userLogin.Password))
            {
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    // EL CONTENIDO DEL JWT
                    Claims = new Dictionary<string, object>
                    {
                        { "id", user.Id },
                        { ClaimTypes.Role, "admin" } // TODO: CAMBIAR ESTO
                    },
                    Expires = DateTime.UtcNow.AddYears(3),
                    SigningCredentials = new SigningCredentials(
                        _tokenParameters.IssuerSigningKey,
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
                string stringToken = tokenHandler.WriteToken(token);
                return Ok(stringToken);
            }
            else
            {
                return Unauthorized();
            }
            
        }
    }
}
