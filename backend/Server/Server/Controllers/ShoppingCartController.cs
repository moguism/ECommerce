using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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


        public ShoppingCartController(UnitOfWork unitOfWork) 
        { 
            _unitOfWork = unitOfWork;
        }



        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<ShoppingCart>> GetOrders()
        {
            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return null;
            }

            return user.ShoppingCart; // Los pedidos normales
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
