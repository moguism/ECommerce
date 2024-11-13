using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Mappers;
using Server.Models;
using Server.Services;
using Stripe.Checkout;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CheckoutController : ControllerBase
{

    private readonly Settings _settings;
    private readonly CartContentMapper _cartContentMapper;
    private readonly ShoppingCartService _shoppingCartService;
    // TODO: Quitar esto
    private readonly UnitOfWork _unitOfWork;

    public CheckoutController(Settings settings, CartContentMapper cartContentMapper, ShoppingCartService shoppingCartService, UnitOfWork unitOfWork)
    {
        _settings = settings;
        _cartContentMapper = cartContentMapper;
        _shoppingCartService = shoppingCartService;
        _unitOfWork = unitOfWork;
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
    public async Task<ActionResult> EmbededCheckout()
    {
        User user = await GetAuthorizedUser();
        if (user == null)
            Unauthorized("Usuario no autenticado.");

        Session session = await GetOptions(user, "embedded");

        return Ok(new { clientSecret = session.ClientSecret });
        //return Ok(new { sessionId = session.Id });

    }

    [HttpPost("hosted")]
    public async Task<ActionResult> HostedCheckout()
    {
        User user = await GetAuthorizedUser();
        if (user == null)
            Unauthorized("Usuario no autenticado.");


        Session session = await GetOptions(user, "hosted");

        return Ok(new { sessionUrl = session.Url });
    }

    private async Task<Session> GetOptions(User user, string mode)
    {
        ShoppingCart shoppingCart = await _shoppingCartService.GetShoppingCartByUserIdAsync(user.Id, true);
        IEnumerable<CartContent> cartContents = shoppingCart.CartContent;
        if (cartContents == null || !cartContents.Any())
            return null;

        var lineItems = new List<SessionLineItemOptions>();

        foreach (CartContent cartContent in cartContents)
        {
            Product product = await _unitOfWork.ProductRepository.GetFullProductById(cartContent.ProductId);
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
                        Images = new List<string> { product.Image }
                    }
                },
                Quantity = cartContent.Quantity
            };

            lineItems.Add(lineItem);

        }

        // Configurar la sesión de pago con los LineItems del carrito
        SessionCreateOptions options = new SessionCreateOptions
        {
            UiMode = mode,
            Mode = "payment",
            PaymentMethodTypes = ["card"],
            LineItems = lineItems, //Lista de productos del usuario
            CustomerEmail = user.Email
        };

        if(mode.Equals("embedded"))
        {
            options.ReturnUrl = _settings.ClientBaseUrl + "/checkout?session_id={CHECKOUT_SESSION_ID}";
        }
        else
        {
            options.SuccessUrl = _settings.ClientBaseUrl + "/checkout?session_id={CHECKOUT_SESSION_ID}";
            options.CancelUrl = _settings.ClientBaseUrl + "/checkout";
        }

        SessionService service = new SessionService();
        Session session = await service.CreateAsync(options);
        return session;
    }

    //Verifica el estado de la sesión
    [HttpGet("status/{sessionId}")]
    public async Task<ActionResult> SessionStatus(string sessionId)
    {
        SessionService sessionService = new SessionService();
        Session session = await sessionService.GetAsync(sessionId);

        return Ok(new { status = session.Status, customerEmail = session.CustomerEmail });
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
