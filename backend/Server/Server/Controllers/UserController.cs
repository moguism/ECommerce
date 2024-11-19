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
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<UserAfterLoginDto>> GetAllUsers()
        {
            User user = await GetCurrentUser();
            
            if(user == null || !user.Role.Equals("Admin"))
            {
                return null;
            }

            //Obtiene todos los usuarios
            ICollection<User> users = await _unitOfWork.UserRepository.GetAllAsync();
            users.Remove(user); // Borra de la lista al propio usuario

            //Paso a DTO
            IEnumerable<UserAfterLoginDto> userDtos = _userMapper.ToDto(users);

            return userDtos;
        }

        [Authorize]
        [HttpGet("authorizedUser")]
        public async Task<UserAfterLoginDto> GetAuthorizedUser()
        {
            User user = await GetCurrentUser();
            if(user == null)
            {
                return null;
            }

            UserAfterLoginDto userDto = _userMapper.ToDto(user);

            return userDto;
        }

        [Authorize]
        [HttpDelete]
        public async Task DeleteUser([FromQuery] int id)
        {
            User user = await GetCurrentUser();
            if (user == null || !user.Role.Equals("Admin") || user.Id == id)
            {
                return;
            }

            User deletedUser = await _userService.GetUserById(id);

            if (deletedUser == null)
            {
                return;
            }

            await _userService.DeleteUser(deletedUser);
        }

        /*[HttpGet("byemail")]
        public async Task<UserAfterLoginDto> GetUserByEmail(string email)
        {
            User user = await _unitOfWork.UserRepository.GetByEmailAsync(email);


            //Paso a DTO
            UserAfterLoginDto userDto = _userMapper.ToDto(user);

            return userDto;
        }*/

        private async Task<User> GetCurrentUser()
        {
            // Pilla el usuario autenticado según ASP
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            string idString = currentUser.Claims.First().ToString().Substring(3); // 3 porque en las propiedades sale "id: X", y la X sale en la tercera posición

            // Pilla el usuario de la base de datos
            return await _userService.GetUserFromDbByStringId(idString);
        }

    }
}
