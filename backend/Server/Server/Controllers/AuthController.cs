using Microsoft.AspNetCore.Mvc;
using Server.Mappers;
using Server.Models;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserMapper _userMapper;
        private readonly UnitOfWork _unitOfWork;
        private readonly PasswordService _passwordService;

        public AuthController(UnitOfWork unitOfWork, UserMapper userMapper, PasswordService passwordService)
        {
            _unitOfWork = unitOfWork;
            _userMapper = userMapper;
            _passwordService = passwordService;
        }

        [HttpPost]
        public async Task RegisterUserAsync([FromBody] UserSignUpDto receivedUser)
        {

            User user = new User();

            ICollection<User> users =  await _unitOfWork.UserRepository.GetAllAsync();
            user.Id = users.Count() == 0 ? 1 : users.Max(u => u.Id) + 1;

            user.Name = receivedUser.Name;
            user.Email = receivedUser.Email;
            user.Password = _passwordService.Hash(receivedUser.Password);
            user.Address = receivedUser.Address;
            user.Role = "admin"; // ESTO HABRÁ QUE CAMBIARLO
            await _unitOfWork.UserRepository.InsertAsync(user);
            await _unitOfWork.SaveAsync();
        }
    }
}
