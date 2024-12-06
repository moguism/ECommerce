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
    public class ShoppingCartController : ControllerBase
    {
        private readonly ShoppingCartService _shoppingCartService;

        public ShoppingCartController(ShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ShoppingCartDto> GetShoppingCart()
        {
            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return null;
            }

            ShoppingCart shoppingCart = user.ShoppingCart;
            if (shoppingCart == null)
            {
                return null;
            }

            ShoppingCartMapper shoppingCartMapper = new ShoppingCartMapper();
            return shoppingCartMapper.ToDto(shoppingCart);
        }

        [Authorize]
        [HttpPost("addProductOrChangeQuantity")]
        public async Task<int> AddProductosToShoppingCart([FromBody] CartContent cartContent, [FromQuery] bool add)
        {

            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return 0;
            }

            if (cartContent.Quantity <= 0)
            {
                return 0;
            }

            ShoppingCart cart = user.ShoppingCart;

            if(cart == null)
            {
                return 0;
            }

            await _shoppingCartService.AddProductsToShoppingCart(user, cartContent, add);
            return user.ShoppingCart.CartContent.Count;
        }

        [Authorize]
        [HttpDelete]
        public async Task RemoveProductFromShoppingCart([FromQuery] int productId)
        {

            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return;
            }

            await _shoppingCartService.RemoveProductFromShoppingCart(user, productId);

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