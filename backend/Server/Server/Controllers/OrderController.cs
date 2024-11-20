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


    //Hay que poner Services.OrderService porque da conflictos en Stripe
    private readonly UserService _userService;
    private readonly Services.OrderService _orderService;
    private readonly Services.ProductService _productService;
    private readonly WishListService _wishListService;
    private readonly ProductsToBuyMapper _productsToBuyMapper;


    public OrderController(UserService userService, Services.OrderService orderService, 
        WishListService wishListService, ProductsToBuyMapper productsToBuyMapper,
        Services.ProductService productService)
    {
        _userService = userService;
        _orderService = orderService;
        _wishListService = wishListService;
        _productsToBuyMapper = productsToBuyMapper;
        _productService = productService;
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

    /*[Authorize]
    [HttpGet("lastOrder")]
    public async Task<Models.Order> GetLastOrder([FromQuery] string id)
    {
        User user = await GetCurrentUser();

        if (user == null)
        {
            return null;
        }

        Models.Order order = await _orderService.GetByPaymentId(id);

        return order;
    }*/


    private async Task<User> GetCurrentUser()
    {
        // Pilla el usuario autenticado según ASP
        System.Security.Claims.ClaimsPrincipal currentUser = this.User;
        string idString = currentUser.Claims.First().ToString().Substring(3); // 3 porque en las propiedades sale "id: X", y la X sale en la tercera posición

        // Pilla el usuario de la base de datos
        return await _userService.GetUserFromDbByStringId(idString);
    }
}
