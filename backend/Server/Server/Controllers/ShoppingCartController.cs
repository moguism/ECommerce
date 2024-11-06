using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Mappers;
using Server.Models;
using Server.Repositories;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {

        private readonly UnitOfWork _unitOfWork;

        private readonly ShoppingCartMapper _shoppingCartMapper;
        private readonly ShoppingCartRepository _shoppingCartRepository;


        public ShoppingCartController(UnitOfWork unitOfWork, ShoppingCartMapper shoppingCartMapper, 
            ShoppingCartRepository shoppingCartRepository) 
        { 
            _unitOfWork = unitOfWork;
            _shoppingCartMapper = shoppingCartMapper;
            _shoppingCartRepository = shoppingCartRepository;
        }


        /* Correcto
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<ShoppingCart>> GetShoppingCartProducts()
        {
            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return null;
            }


            var shoppingCart = await _context.ShoppingCart
            .Where(cart => cart.UserId == user.Id)  // Filtra por el ID del usuario
            .ToListAsync();

            return shoppingCart;


        }
        */

        //Pruebas
        [HttpGet]
        public async Task<ShoppingCartDto> GetShoppingCart(int userId)
        {

            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetQueryable().Where(cart => cart.UserId == userId).FirstOrDefaultAsync();

            //Devuelve el contenido del carrito (Productos y cantidad)
            return _shoppingCartMapper.ToDto(shoppingCart);
          
        }

        [HttpPost]
        public async Task AddProductosToShoppingCart([FromBody] CartContentDto cartContentDto, [FromQuery] User user)
        {

            bool existShoppingCart = await _shoppingCartRepository.AddNewShoppingCart(user);

            


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
