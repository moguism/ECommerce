using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Mappers;
using Server.Models;
using Server.Services;
using Stripe.Climate;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{

    UserService _userService;
    //Hay que poner Services.OrderService porque da conflictos en Stripe
    Services.OrderService _orderService;
    WishListService _wishListService;
    ProductsToBuyMapper _productsToBuyMapper;


    public OrderController(UserService userService, Services.OrderService orderService, 
        WishListService wishListService, ProductsToBuyMapper productsToBuyMapper)
    {
        _userService = userService;
        _orderService = orderService;
        _wishListService = wishListService;
        _productsToBuyMapper = productsToBuyMapper;
    }


    [Authorize]
    [HttpGet("allUserOrders")]
    public async Task<IEnumerable<Models.Order>> GetAllOrders()
    {
        User user = await GetCurrentUser();

        if (user == null)
        {
            return null;
        }

        return await _orderService.GetAllOrders(user);
    }

    [Authorize]
    [HttpGet("lastOrder")]
    public async Task<Models.Order> GetLastOrder()
    {
        User user = await GetCurrentUser();

        if (user == null)
        {
            return null;
        }

        return await _orderService.GetOrderById(user.Orders.Last().Id);
    }


    private async Task<User> GetCurrentUser()
    {
        // Pilla el usuario autenticado según ASP
        System.Security.Claims.ClaimsPrincipal currentUser = this.User;
        string idString = currentUser.Claims.First().ToString().Substring(3); // 3 porque en las propiedades sale "id: X", y la X sale en la tercera posición

        // Pilla el usuario de la base de datos
        return await _userService.GetUserFromDbByStringId(idString);
    }
}
