using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Server.DTOs;
using Server.Mappers;
using Server.Models;
using Server.Repositories;
using Server.Repositories.Base;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserMapper _userMapper;
        private readonly UnitOfWork _unitOfWork;
        private readonly UserService _userService;

        public UserController(UnitOfWork unitOfWork, UserMapper userMapper, UserService userService)
        {
            _unitOfWork = unitOfWork;
            _userMapper = userMapper;
            _userService = userService;
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

        [HttpGet("authorizedUser")]
        public async Task<User> GetAuthorizedUser()
        {
            // Pilla el usuario autenticado según ASP
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            string idString = currentUser.Claims.First().ToString().Substring(3); // 3 porque en las propiedades sale "id: X", y la X sale en la tercera posición

            // Pilla el usuario de la base de datos
            return await _userService.GetUserFromDbByStringId(idString);
        }

    }
}
