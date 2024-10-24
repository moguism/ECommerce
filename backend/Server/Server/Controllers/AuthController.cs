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
            ([FromBody] User user)
        {

            User usuario = new User();
            usuario.Id = _context.Users.Count() == 0 ? 1 : _context.Users.Max(u => u.Id) + 1;
            usuario.Name = user.Name;
            usuario.Email = user.Email;
            usuario.Password = user.Password;
            usuario.Address = user.Address;
            usuario.Role = user.Role;
            await _unitOfWork.UserRepository.InsertAsync(usuario);
            await _unitOfWork.SaveAsync();
        }
    }
}
