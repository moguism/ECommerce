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
        private readonly TemporalOrderMapper _temporalOrderMapper;
        private readonly ShoppingCartService _shoppingCartService;
        private readonly TemporalOrderService _temporalOrderService;

        public TemporalOrderController(ShoppingCartMapper shoppingCartMapper, TemporalOrderService temporalOrderService, ShoppingCartService shoppingCartService, TemporalOrderMapper temporalOrderMapper)
        {
            _shoppingCartMapper = shoppingCartMapper;
            _temporalOrderService = temporalOrderService;
            _shoppingCartService = shoppingCartService;
            _temporalOrderMapper = temporalOrderMapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<TemporalOrderDto> GetTemporalOrderById([FromQuery] int id)
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

            return _temporalOrderMapper.ToDto(temporalOrder);
        }

        [Authorize]
        [HttpPost]
        public async Task<TemporalOrderDto> CreateTemporalOrder()
        {

            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return null;
            }

            ShoppingCart cart = await _shoppingCartService.GetShoppingCartByUserIdAsync(user.Id, false);

            if(cart == null)
            {
                return null;
            }

            cart = await _shoppingCartService.ChangeTemporalAttribute(cart, true); // Pone el carrito como temporal

            return await CreateTemporal(cart, user);
        }

        [Authorize]
        [HttpPost("direct")]
        public async Task<TemporalOrderDto> CreateDirectTemporalOrder([FromBody] IEnumerable<CartContentDto> cartContent) // Le paso un array de contenido de carrito porque habrá varios productos
        {
            User user = await GetAuthorizedUser();
            if(user == null)
            {
                return null;
            }

            ShoppingCart cart = await _shoppingCartService.CreateShoppingCart(user, true); // Le creo un nuevo carro, aunque quizás no es la mejor opción
            await _temporalOrderService.AddDirectTemporalOrder(cartContent, cart);
            return await CreateTemporal(cart, user);
        }

        [Authorize]
        [HttpGet("refresh")]
        public async Task RefreshTemporalOrder([FromQuery] int id)
        {
            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return;
            }

            TemporalOrder temporalOrder = await _temporalOrderService.GetFullTemporalOrderById(id);

            if (temporalOrder.ShoppingCart.UserId != user.Id)
            {
                return;
            }

            await _temporalOrderService.UpdateExpiration(temporalOrder);
        }

        private async Task<TemporalOrderDto> CreateTemporal(ShoppingCart cart, User user)
        {
            TemporalOrder temporalOrder = new TemporalOrder();
            //temporalOrder.UserId = user.Id;
            temporalOrder.ShoppingCartId = cart.Id;
            temporalOrder.ExpirationDate = DateTime.UtcNow;

            TemporalOrder savedTemporalOrder = await _temporalOrderService.CreateTemporalOrder(temporalOrder, user);
            return _temporalOrderMapper.ToDto(savedTemporalOrder);
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