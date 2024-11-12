using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Mappers;
using Server.Models;
using Server.Services;
using Stripe.Checkout;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {

        private readonly Settings _settings;
        private readonly CartContentMapper _cartContentMapper;
        private readonly ShoppingCartService _shoppingCartService;

        public CheckoutController(Settings settings, CartContentMapper cartContentMapper, ShoppingCartService shoppingCartService)
        {
            _settings = settings;
            _cartContentMapper = cartContentMapper;
            _shoppingCartService = shoppingCartService;
        }

        public async Task<IEnumerable<CartContent>> GetCartContent(IEnumerable<CartContentDto> cartContentDtos, User user) {

            if (user == null)
            {
                return null;
            }

            ShoppingCart shoppingCart = await _shoppingCartService.GetShoppingCartByUserIdAsync(user.Id);

            return _cartContentMapper.ToEntity(cartContentDtos, shoppingCart);
        }


        //Crea la sesión del pago
        [Authorize]
        [HttpGet("hosted")]
        public async Task<ActionResult> HostedCheckout(CartContentDto[] cartContentDto)
        {

            User user = await GetAuthorizedUser();

            IEnumerable<CartContent> cartContents = await GetCartContent(cartContentDto, user);

            if (cartContents == null)
            {
                return null;
            }


            SessionCreateOptions options = new SessionCreateOptions
            {
                UiMode = "hosted",
                Mode = "payment",
                PaymentMethodTypes = ["card"],
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        Currency = "eur",
                        UnitAmount = (long)(product.Price * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = product.Name,
                            Description = product.Description,
                            Images = [product.ImageUrl]
                        }
                    },
                    Quantity = 1,
                },
            },
                CustomerEmail = "correo_cliente@correo.es",
                SuccessUrl = _settings.ClientBaseUrl + "/checkout?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = _settings.ClientBaseUrl + "/checkout"
            };

            SessionService service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Ok(new { sessionUrl = session.Url });
        }


        //Iniciar Sesión de Pago (Modo Embebido)
        [HttpGet("embedded")]
        public async Task<ActionResult> EmbededCheckout()
        {
            ProductDto product = GetProducts()[0];

            SessionCreateOptions options = new SessionCreateOptions
            {
                UiMode = "embedded",
                Mode = "payment",
                PaymentMethodTypes = ["card"],
                LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        Currency = "eur",
                        UnitAmount = (long)(product.Price * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = product.Name,
                            Description = product.Description,
                            Images = [product.ImageUrl]
                        }
                    },
                    Quantity = 1,
                },
            },
                CustomerEmail = "correo_cliente@correo.es",
                ReturnUrl = _settings.ClientBaseUrl + "/checkout?session_id={CHECKOUT_SESSION_ID}",
            };

            SessionService service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Ok(new { clientSecret = session.ClientSecret });
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
}
