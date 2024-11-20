using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Models;
using Server.Services;
using Stripe.Climate;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{

    UserService _userService;
    Services.OrderService _orderService;

    public OrderController(UserService userService, Services.OrderService orderService)
    {
        _userService = userService;
        _orderService = orderService;
    }


    [Authorize]
    [HttpGet("allUserOrders")]
    public async Task<IEnumerable<Models.Order>> GetAllOrders()
    {
        User user = await GetCurrentUser();

        if (user == null || !user.Role.Equals("Admin"))
        {
            return null;
        }

        return await _orderService.GetAllOrders(user);
    }


    [HttpGet("allProductsByOrderId")]
    public async Task<IEnumerable<Models.Order>> GetAllProducts(int orderId)
    {
        return await _orderService.GetAllOrders(user);
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
