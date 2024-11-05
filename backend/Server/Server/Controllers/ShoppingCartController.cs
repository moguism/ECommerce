using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Mappers;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {

        private readonly UnitOfWork _unitOfWork;
        private readonly FarminhouseContext _context;


        public ShoppingCartController(UnitOfWork unitOfWork, FarminhouseContext context) 
        { 
            _unitOfWork = unitOfWork;
            _context = context;
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
        public async Task<IEnumerable<ShoppingCart>> GetShoppingCartProducts(int id)
        {


            var shoppingCart = await _context.ShoppingCart
            .Where(cart => cart.UserId == id)  // Filtra por el ID del usuario
            .Include
            .ToListAsync();

            return shoppingCart;


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
