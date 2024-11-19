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

    // TODO: QUITAR LA UNIT OF WORK

    private readonly Settings _settings;
    private readonly CartContentMapper _cartContentMapper;
    private readonly ShoppingCartService _shoppingCartService;
    private readonly OrderService _orderService;
    private readonly EmailService _emailService;
    private readonly UnitOfWork _unitOfWork;

    public CheckoutController(Settings settings, CartContentMapper cartContentMapper, 
        ShoppingCartService shoppingCartService, UnitOfWork unitOfWork, 
        OrderService orderService,EmailService emailService)
    {
        _settings = settings;
        _cartContentMapper = cartContentMapper;
        _shoppingCartService = shoppingCartService;
        _unitOfWork = unitOfWork;
        _orderService = orderService;
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

        TemporalOrder temporalOrder = await GetTemporal(temporalOrderId, user);

        if(temporalOrder == null)
        {
            return null;
        }

        Session session = await GetOptions(temporalOrder, "embedded", user);

        return Ok(new { clientSecret = session.ClientSecret });
        //return Ok(new { sessionId = session.Id });

    }

    [HttpPost("hosted")]
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
    }

    private async Task<TemporalOrder> GetTemporal(int id, User user)
    {
        TemporalOrder temporalOrder = await _unitOfWork.TemporalOrderRepository.GetFullTemporalOrderById(id);

        if (temporalOrder.UserId != user.Id)
        {
            return null;
        }

        return temporalOrder;
    }

    private async Task<Session> GetOptions(TemporalOrder temporalOrder, string mode, User user)
    {
        /*ShoppingCart shoppingCart = await _shoppingCartService.GetShoppingCartByUserIdAsync(user.Id);
        IEnumerable<CartContent> cartContents = shoppingCart.CartContent;
        if (cartContents == null || !cartContents.Any())
            return null;*/

        var lineItems = new List<SessionLineItemOptions>();

        Wishlist wishlist = await _unitOfWork.WishlistRepository.GetFullByIdAsync(temporalOrder.WishlistId);

        foreach (ProductsToBuy cartContent in wishlist.Products)
        {
            var product = await _unitOfWork.ProductRepository.GetFullProductById(cartContent.ProductId);
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
            options.ReturnUrl = _settings.ClientBaseUrl + "/after-checkout?session_id={CHECKOUT_SESSION_ID}";
        }
        else
        {
            options.SuccessUrl = _settings.ClientBaseUrl + "/after-checkout?session_id={CHECKOUT_SESSION_ID}";
            options.CancelUrl = _settings.ClientBaseUrl + "/after-checkout";
        }

        SessionService service = new SessionService();
        Session session = await service.CreateAsync(options);
        return session;
    }

    //Verifica el estado de la sesión
    [HttpGet("status/{sessionId}")]
    public async Task<Order> SessionStatus(string sessionId)
    {
        SessionService sessionService = new SessionService();
        Session session = await sessionService.GetAsync(sessionId);
        if (session.PaymentStatus == "paid")
        {
            Order order= await _orderService.CompletePayment(session);
            if (session.CustomerEmail != null)
            {
                string to = session.CustomerEmail;
                String subject = "Envio de la compra realizada";
                String body = "Gracias por comprar en Farm in house a salavdo a millones de animales con esta compra";
                await _emailService.SendEmailAsync(to, subject, body,false);
            }
            return order;
        }
        return null;
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
