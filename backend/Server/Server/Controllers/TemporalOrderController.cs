using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Mappers;
using Server.Models;
using Server.Repositories;
using Server.Services;
using TorchSharp;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemporalOrderController : ControllerBase
    {
        private readonly TemporalOrderMapper _temporalOrderMapper;
        private readonly ShoppingCartService _shoppingCartService;
        private readonly TemporalOrderService _temporalOrderService;
        private readonly WishListService _wishListService; 

        public TemporalOrderController(TemporalOrderService temporalOrderService, 
            ShoppingCartService shoppingCartService, TemporalOrderMapper temporalOrderMapper,
            WishListService wishListService)
        {
            _temporalOrderService = temporalOrderService;
            _shoppingCartService = shoppingCartService;
            _temporalOrderMapper = temporalOrderMapper;
            _wishListService = wishListService;
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
            Wishlist wishlist = await _wishListService.GetWishlistByIdAsync(temporalOrder.WishlistId);
            if(temporalOrder == null || wishlist == null)
            {
                return null;
            }

            temporalOrder.Wishlist = wishlist;

            return _temporalOrderMapper.ToDto(temporalOrder);
        }


        [Authorize]
        [HttpPost("newTemporalOrder")]
        public async Task<TemporalOrder> CreateTemporal([FromBody] TemporalOrderDto temporalOrderDto)
        {
            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return null;
            }


            Wishlist wishlist = await _wishListService.CreateNewWishList(temporalOrderDto.CartContentDtos);// Añade a la nueva wislist los productos que el usuario quire comprar

            //Añade una nueva orden temporal con los datos del usuario
            TemporalOrder order = await _temporalOrderService.CreateTemporalOrder(user,wishlist,temporalOrderDto.Quick);
            return order;

        }

        [Authorize]
        [HttpGet("refresh")]
        public async Task<TemporalOrderDto> RefreshTemporalOrder([FromQuery] int id)
        {
            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return null;
            }

            TemporalOrder temporalOrder = await _temporalOrderService.GetFullTemporalOrderById(id);

            if (temporalOrder == null || temporalOrder.UserId != user.Id)
            {
                return null;
            }

            await _temporalOrderService.UpdateExpiration(temporalOrder);

            return _temporalOrderMapper.ToDto(temporalOrder);
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