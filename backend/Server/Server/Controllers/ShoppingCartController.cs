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

        private readonly CartContentMapper _cartContentMapper;


        public ShoppingCartController(UnitOfWork unitOfWork, CartContentMapper cartContentMapper) 
        { 
            _unitOfWork = unitOfWork;
            _cartContentMapper = cartContentMapper;
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
        public async Task<IEnumerable<CartContentDto>> GetShoppingCart(int userId)
        {

            var shoppingCart = await _unitOfWork.ShoppingCartRepository.GetQueryable().Where(cart => cart.UserId == userId) .FirstOrDefaultAsync();

            //Devuelve el contenido del carrito (Productos y cantidad)
            return await _cartContentMapper.ToDto(shoppingCart);


            return shoppingCart;


            //Devuelve el contenido del carrito (Productos y cantidad)
            return await _cartContentMapper.ToDto(shoppingCart);




            // Verificar si existe un carrito del usuario
            var existingShoppingCar = await _unitOfWork.ShoppingCartRepository.GetQueryable().Where(cart => cart.Id == cartContentDto.Id).FirstOrDefaultAsync();


            //Si no hay ningún carrito creado por el usuario, lo crea
            if (existingShoppingCar == null)
            {
                await _unitOfWork.ShoppingCartRepository.InsertAsync(new ShoppingCart() { UserId = user.Id });
            }
            else
            {
                //Si existe el carrito, añade el producto a este
                await _unitOfWork.CartContentRepository.AddProductToCartAsync(_cartContentMapper.ToEntity(cartContentDto));

        [HttpPost]
        public async Task AddProductosToShoppingCart([FromBody] CartContentDto cartContentDto, [FromQuery] User user)
        {

            // Verificar si existe un carrito del usuario
            var existingShoppingCar = await _context.ShoppingCart
                .FirstOrDefaultAsync(cart => cart.Id == cartContentDto.Id);

            //Si no hay ningún carrito creado por el usuario, lo crea
            if (existingShoppingCar == null)
            {
                _context.ShoppingCart.Add(new ShoppingCart() { UserId = user.Id});
            }
            else
            {
                //Si existe el carrito, añade el producto a este
                await _cartContentRepository.AddProductToCartAsync(_cartContentMapper.ToEntity(cartContentDto));

            }

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
