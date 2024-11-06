using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Server.DTOs;
using Server.Mappers;
using Server.Models;
using Server.Repositories;
using Server.Repositories.Base;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserMapper _userMapper;
        private readonly UnitOfWork _unitOfWork;

        public UserController(UnitOfWork unitOfWork, UserMapper userMapper)
        {
            _unitOfWork = unitOfWork;
            _userMapper = userMapper;
        }



        //Obtiene todos los usuarios sin contraseña
        [HttpGet]
        public async Task<IEnumerable<UserAfterLoginDto>> GetAllUsers()
        {
            //Obtener todos los usuarios
            ICollection<User> users = await _unitOfWork.UserRepository.GetAllAsync();


            //Paso a DTO
            IEnumerable<UserAfterLoginDto> userDtos = _userMapper.ToDto(users);

            return userDtos;
        }

        
        [HttpGet("byemail")]
        public async Task<UserAfterLoginDto> GetUserByEmail(string email)
        {
            User user = await _unitOfWork.UserRepository.GetByEmailAsync(email);


            //Paso a DTO
            UserAfterLoginDto userDto = _userMapper.ToDto(user);

            return userDto;
        }
        
        
    }
}
