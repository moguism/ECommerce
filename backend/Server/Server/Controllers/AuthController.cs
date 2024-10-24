using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Mappers;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserMapper _userMapper;
        private readonly UnitOfWork _unitOfWork;
        FarminhouseContext _context;

        public AuthController(UnitOfWork unitOfWork, UserMapper userMapper, FarminhouseContext context)
        {
            _unitOfWork = unitOfWork;
            _userMapper = userMapper;
            _context = context;
        }




        [HttpPost]
        public async Task RegisterUserAsync
            (string name, string email, string password, string address)
        {

            User usuario = new User();
            usuario.Id = _context.Users.Count() == 0 ? 1 : _context.Users.Max(u => u.Id) + 1;
            usuario.Name = name;
            usuario.Email = email;
            usuario.Password = password;
            usuario.Address = address;
            usuario.Role = "normal";
            await _unitOfWork.UserRepository.InsertAsync(usuario);
            await _unitOfWork.SaveAsync();
        }
    }
}
