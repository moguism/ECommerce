using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Mappers;
using Server.Models;
using Server.Repositories;
using Server.Services;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemporalOrderController : ControllerBase
    {

        private readonly ShoppingCartMapper _shoppingCartMapper;
        private readonly ShoppingCartService _shoppingCartService;
        private readonly TemporalOrderService _temporalOrderService;

        public TemporalOrderController(ShoppingCartMapper shoppingCartMapper, TemporalOrderService temporalOrderService, ShoppingCartService shoppingCartService)
        {
            _shoppingCartMapper = shoppingCartMapper;
            _temporalOrderService = temporalOrderService;
            _shoppingCartService = shoppingCartService;
        }

        [Authorize]
        [HttpPost]
        public async Task<TemporalOrder> CreateTemporalOrder()
        {

            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return null;
            }

            ShoppingCart cart = await _shoppingCartService.GetShoppingCartByUserIdAsync(user.Id);

            TemporalOrder temporalOrder = new TemporalOrder();
            temporalOrder.UserId = user.Id;
            temporalOrder.ShoppingCartId = cart.Id;

            TemporalOrder savedTemporalOrder = await _temporalOrderService.CreateTemporalOrder(temporalOrder);
            return savedTemporalOrder;
        }

        private async Task<User> GetAuthorizedUser()
        {
            // Pilla el usuario autenticado según ASP
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            string idString = currentUser.Claims.First().ToString().Substring(3); // 3 porque en las propiedades sale "id: X", y la X sale en la tercera posición

            // Pilla el usuario de la base de datos
            return await _shoppingCartService.GetUserFromDbByStringId(idString);
        }

    }
}