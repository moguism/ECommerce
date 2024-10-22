using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        FarminhouseContext _context;

        public UserController(UnitOfWork unitOfWork, UserMapper userMapper, FarminhouseContext context)
        {
            _unitOfWork = unitOfWork;
            _userMapper = userMapper;
            _context = context;
        }



        //Obtiene todos los usuarios sin contraseña
        [HttpGet]
        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            //Obtener todos los usuarios
            ICollection<User> users = await _unitOfWork.UserRepository.GetAllAsync();


            //Paso a DTO
            IEnumerable<UserDto> userDtos = _userMapper.ToDto(users);

            return userDtos;
        }

        [HttpPost]
        public async Task RegisterUserAsync 
            (string name, string email,string password, string address)
        {
            
            User usuario = new User();
            usuario.Id = _context.Users.Count() == 0 ? 1 : _context.Users.Max(u => u.Id);
            usuario.Name = name;
            usuario.Email = email;
            usuario.Password = password;
            usuario.Address = address;

        }


    }
}
