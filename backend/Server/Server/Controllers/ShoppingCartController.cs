﻿using Microsoft.AspNetCore.Authorization;
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
        private readonly ProductService _productService;

        public ShoppingCartController(ShoppingCartMapper shoppingCartMapper, ShoppingCartService shoppingCartService, UnitOfWork unitOfWork, ProductService productService)
        {
            _shoppingCartMapper = shoppingCartMapper;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
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

            ShoppingCart shoppingCart = await _shoppingCartService.GetShoppingCartByUserIdAsync(user.Id);
            if (shoppingCart == null)
            {
                return null;
            }

            return _shoppingCartMapper.ToDto(shoppingCart);


        }

        [Authorize]
        [HttpPost("addProductOrChangeQuantity")]
        public async Task AddProductosToShoppingCart([FromBody] CartContentDto cartContentDto)
        {

            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return;
            }

            Product product = await _productService.GetProductById(cartContentDto.ProductId);
            if(product == null || cartContentDto.Quantity > product.Stock)
            {
                return;
            }

            await _shoppingCartService.AddProductsToShoppingCart(user, cartContentDto);

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