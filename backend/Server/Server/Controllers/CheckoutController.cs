using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Mappers;
using Server.Models;
using Server.Services;
using Stripe;
using Stripe.Checkout;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CheckoutController : ControllerBase
{
    private readonly TemporalOrderService _temporalOrderService;
    private readonly EmailService _emailService;

    public CheckoutController(TemporalOrderService temporalOrderService, EmailService emailService)
    {
        _temporalOrderService = temporalOrderService;
        _emailService = emailService;
    }

    /*public async Task<IEnumerable<CartContent>> GetCartContent(IEnumerable<CartContentDto> cartContentDtos, User user) {

        if (user == null)
        {
            return null;
        }

        ShoppingCart shoppingCart = await _shoppingCartService.GetShoppingCartByUserIdAsync(user.Id, true);

        return _cartContentMapper.ToEntity(cartContentDtos, shoppingCart);
    }*/


    //Iniciar Sesión de Pago (Modo Embebido)
    [Authorize]
    [HttpPost("embedded")]
    public async Task<ActionResult> EmbededCheckout([FromBody] int temporalOrderId)
    {
        User user = await GetAuthorizedUser();
        if (user == null)
            Unauthorized("Usuario no autenticado.");

        TemporalOrder temporalOrder = user.TemporalOrders.FirstOrDefault(t => t.Id == temporalOrderId);

        if (temporalOrder == null)
        {
            return null;
        }

        var lineItems = new List<SessionLineItemOptions>();

        foreach (ProductsToBuy cartContent in temporalOrder.Wishlist.Products)
        {
            var product = cartContent.Product;
            // Crea un SessionLineItemOptions para cada producto en el carrito
            var lineItem = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "eur",
                    UnitAmount = (long)(product.Price),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = product.Name,
                        Description = product.Description,
                        //Images = new List<string> { "" }
                    }
                },
                Quantity = cartContent.Quantity
            };

            lineItems.Add(lineItem);

        }

        // Configurar la sesión de pago con los LineItems del carrito
        SessionCreateOptions options = new SessionCreateOptions
        {
            UiMode = "embedded",
            Mode = "payment",
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = lineItems, //Lista de productos del usuario
            CustomerEmail = user.Email,
            RedirectOnCompletion = "never"
        };

        SessionService service = new SessionService();
        Session session = await service.CreateAsync(options);

        temporalOrder.HashOrSession = session.Id;
        await _temporalOrderService.UpdateTemporalOrder(temporalOrder);

        return Ok(new { clientSecret = session.ClientSecret });
        //return Ok(new { sessionId = session.Id });

    }

    /*[HttpPost("hosted")]
    public async Task<ActionResult> HostedCheckout([FromBody] int temporalOrderId)
    {
        User user = await GetAuthorizedUser();
        if (user == null)
            Unauthorized("Usuario no autenticado.");

        TemporalOrder temporalOrder = await GetTemporal(temporalOrderId, user);

        if(temporalOrder == null)
        {
            return null;
        }

        Session session = await GetOptions(temporalOrder, "hosted", user);

        return Ok(new { sessionUrl = session.Url });
    }*/

    //Verifica el estado de la sesión
    [Authorize]
    [HttpGet("status/{sessionId}")]
    public async Task<Order> SessionStatus(string sessionId)
    {
        SessionService sessionService = new SessionService();
        Session session = await sessionService.GetAsync(sessionId);
        User user = await GetAuthorizedUser();
        if (user == null) 
        {
        Unauthorized("Usuario no autenticado.");
        }

        if (session.PaymentStatus == "paid")
        {
            Order order = await _temporalOrderService.CreateOrderFromTemporal(sessionId, sessionId, user, 1);
            if (session.CustomerEmail != null)
            {
                await _emailService.CreateEmailUser(user, order.Wishlist, order.PaymentTypeId);
            }
            return order;
        }
        return null;
    }

    


    private async Task<User> GetAuthorizedUser(bool all = false)
    {
        // Pilla el usuario autenticado según ASP
        System.Security.Claims.ClaimsPrincipal currentUser = this.User;
        string idString = currentUser.Claims.First().ToString().Substring(3); // 3 porque en las propiedades sale "id: X", y la X sale en la tercera posición

        // Pilla el usuario de la base de datos
        if(!all)
        {
            return await _temporalOrderService.GetUserFromStringWithTemporal(idString);
        }
        else
        {
            return await _temporalOrderService.GetUserFromString(idString);
        }
        
    }
}
