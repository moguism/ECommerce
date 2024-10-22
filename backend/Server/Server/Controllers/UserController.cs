using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public UserController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            //Obtener todos los usuarios
            ICollection<User> users = await _unitOfWork.UserRepository.GetAllAsync();


            //Paso a DTO
            IEnumerable<UserDto> userDtos = _userMapper.ToDto(users);

            return userDtos;
        }


    }
}
