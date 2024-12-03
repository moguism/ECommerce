using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Mappers;
using Server.Models;
using Server.Repositories;
using Server.Services;
using TorchSharp;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemporalOrderController : ControllerBase
    {
        private readonly TemporalOrderService _temporalOrderService;
        private readonly UserService _userService;

        public TemporalOrderController(TemporalOrderService temporalOrderService, UserService userService)
        {
            _temporalOrderService = temporalOrderService;
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public async Task<TemporalOrder> GetTemporalOrderById([FromQuery] int id)
        {
            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return null;
            }

            TemporalOrder temporalOrder = await _temporalOrderService.GetFullTemporalOrderById(id);
            if(temporalOrder == null)
            {
                return null;
            }

            return temporalOrder;
        }


        [Authorize]
        [HttpPost("newTemporalOrder")]
        public async Task<TemporalOrder> CreateTemporal([FromBody] TemporalOrderDto temporalOrderDto)
        {
            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return null;
            }

            //Añade una nueva orden temporal con los datos del usuario
            TemporalOrder order = await _temporalOrderService.CreateTemporalOrder(user,temporalOrderDto.Quick, temporalOrderDto);
            return order;

        }

        [Authorize]
        [HttpGet("refresh")]
        public async Task<TemporalOrder> RefreshTemporalOrder([FromQuery] int id)
        {
            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return null;
            }

            TemporalOrder temporalOrder = await _temporalOrderService.GetFullTemporalOrderById(id);

            if (temporalOrder == null || temporalOrder.UserId != user.Id)
            {
                return null;
            }

            await _temporalOrderService.UpdateExpiration(temporalOrder);

            return temporalOrder;
        }

        private async Task<User> GetAuthorizedUser()
        {
            // Pilla el usuario autenticado según ASP
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            string idString = currentUser.Claims.First().ToString().Substring(3); // 3 porque en las propiedades sale "id: X", y la X sale en la tercera posición

            // Pilla el usuario de la base de datos
            return await _userService.GetUserFromDbByStringId(idString);
        }

    }
}