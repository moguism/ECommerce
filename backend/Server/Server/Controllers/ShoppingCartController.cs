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

        private readonly UnitOfWork _unitOfWork;

        private readonly ShoppingCartMapper _shoppingCartMapper;
        private readonly ShoppingCartRepository _shoppingCartRepository;
        private readonly ShoppingCartService _shoppingCartService;


        public ShoppingCartController(UnitOfWork unitOfWork, ShoppingCartMapper shoppingCartMapper, 
            ShoppingCartRepository shoppingCartRepository, ShoppingCartService shoppingCartService) 
        { 
            _unitOfWork = unitOfWork;
            _shoppingCartMapper = shoppingCartMapper;
            _shoppingCartRepository = shoppingCartRepository;
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

            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetQueryable()
                .Where(cart => cart.UserId == user.Id)
                .FirstOrDefaultAsync();

            
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

            await _shoppingCartRepository.AddNewShoppingCart(user);

            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetAllByUserIdAsync(user.Id);
            
            await _shoppingCartService.AddProductsToShoppingCart(shoppingCart, cartContentDto);

        }




        private async Task<User> GetAuthorizedUser()
        {
            // Pilla el usuario autenticado según ASP
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            string idString = currentUser.Claims.First().ToString().Substring(3); // 3 porque en las propiedades sale "id: X", y la X sale en la tercera posición

            // Pilla el usuario de la base de datos
            return await _unitOfWork.UserRepository.GetAllInfoById(Int32.Parse(idString));
        }

    }
}
