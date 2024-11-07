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

        private readonly ShoppingCartMapper _shoppingCartMapper;
        private readonly ShoppingCartService _shoppingCartService;
        private readonly UnitOfWork _unitOfWork;

        public ShoppingCartController(ShoppingCartMapper shoppingCartMapper, ShoppingCartService shoppingCartService, UnitOfWork unitOfWork) 
        { 
            _shoppingCartMapper = shoppingCartMapper;
            _shoppingCartService = shoppingCartService;
            _unitOfWork = unitOfWork;
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

            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetAllByUserIdAsync(user.Id);

            if (shoppingCart == null)
            {
                return null;
            }

            return _shoppingCartMapper.ToDto(shoppingCart);
          
        }

        [Authorize]
        [HttpPost]
        public async Task AddProductosToShoppingCart([FromBody] CartContentDto cartContentDto)
        {

            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return;
            }

            ShoppingCart cart = await _unitOfWork.ShoppingCartRepository.GetAllByUserIdAsync(user.Id);
            if(cart == null)
            {
                cart = new ShoppingCart();
                cart.UserId = user.Id;
                cart = await _unitOfWork.ShoppingCartRepository.InsertAsync(cart);
                await _unitOfWork.SaveAsync();
            }
            /*if(shoppingCarts.Count() == 0)
            {
                ShoppingCart cart = new ShoppingCart();
                cart.UserId = user.Id;
                shoppingCarts.Add(cart);

                await _unitOfWork.ShoppingCartRepository.InsertAsync(cart);
                await _unitOfWork.SaveAsync();

                user.ShoppingCarts.Add(cart);
                _unitOfWork.UserRepository.Update(user);

                await _unitOfWork.SaveAsync();
            }
            else if(shoppingCarts.Count() > 1)
            {
                throw new Exception("MAMONAZOS, SOLO UN CARRO POR USUARIO");
            }*/

            //Añade un carrito si el usuario no tiene ninguno
            //await _shoppingCartService.AddNewShoppingCartByUserAsync(user);
            //Recoge el carrito del usuario
            //ShoppingCart shoppingCart = shoppingCarts.FirstOrDefault();
            //Añade los productos al carrito

            CartContent cartContent = new CartContent();
            cartContent.ProductId = cartContentDto.ProductId;
            cartContent.Quantity = cartContentDto.Quantity;
            cartContent.ShoppingCartId = cart.Id;

            await _unitOfWork.CartContentRepository.InsertAsync(cartContent);
            await _unitOfWork.SaveAsync();
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
