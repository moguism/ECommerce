using Bogus;
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

    //Iniciar Sesión de Pago (Modo Embebido)
    [Authorize]
    [HttpPost("embedded")]
    public async Task<ActionResult> EmbededCheckout([FromBody] int temporalOrderId)
    {
       User user = await GetMinimumUser();
        if (user == null)
            Unauthorized("Usuario no autenticado.");

        TemporalOrder temporalOrder = await _temporalOrderService.GetFullTemporalOrderById(temporalOrderId);

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
    [HttpGet("status/{temporalOrderId}")]
    public async Task<Order> SessionStatus(int temporalOrderId)
    {
        User user = await GetAuthorizedUserWithCart();
        if (user == null)
        {
            Unauthorized("Usuario no autenticado.");
        }

        TemporalOrder temporalOrder = await _temporalOrderService.GetFullTemporalOrderById(temporalOrderId);
        if (temporalOrder == null)
        {
            return null;
        }

        SessionService sessionService = new SessionService();
        Session session = await sessionService.GetAsync(temporalOrder.HashOrSession);

        if (session == null || !session.PaymentStatus.Equals("paid", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        Order order = await _temporalOrderService.CreateOrderFromTemporal(temporalOrder, user, 1);
        if (session.CustomerEmail != null)
        {
            await _emailService.CreateEmailUser(user, order.Wishlist, order.PaymentTypeId);
        }
        return order;
    }

    private async Task<User> GetAuthorizedUserWithCart()
    {
        string idString = GetStringId();
        return await _temporalOrderService.GetMinimumWithCart(idString);
    }

    private string GetStringId()
    {
        // Pilla el usuario autenticado según ASP
        System.Security.Claims.ClaimsPrincipal currentUser = this.User;
        string idString = currentUser.Claims.First().ToString().Substring(3); // 3 porque en las propiedades sale "id: X", y la X sale en la tercera posición
        return idString;
    }
    private async Task<User> GetMinimumUser()
    {
        User user = await _temporalOrderService.GetMinimumUser(GetStringId());
        return user;
    }
}
