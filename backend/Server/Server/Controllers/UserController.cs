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
        private readonly Repository _repository


        [HttpGet]
        public IEnumerable<UserDto> GetAllUsers()
        {
            User[] users = _repository.GetAllAsync().Result;

        }


    }
}
