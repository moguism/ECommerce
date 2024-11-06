using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Mappers;
using Server.Models;
using Server.Repositories;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {

        private readonly UnitOfWork _unitOfWork;
        private readonly FarminhouseContext _context;
        private readonly CartContentRepository _cartContentRepository;


        public ShoppingCartController(UnitOfWork unitOfWork, FarminhouseContext context, CartContentRepository cartContentRepository) 
        { 
            _unitOfWork = unitOfWork;
            _context = context;
            _cartContentRepository = cartContentRepository;
        }


        /* Correcto
        [Authorize]
        [HttpGet]
        public async Task<IEnumerable<ShoppingCart>> GetShoppingCartProducts()
        {
            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return null;
            }


            var shoppingCart = await _context.ShoppingCart
            .Where(cart => cart.UserId == user.Id)  // Filtra por el ID del usuario
            .ToListAsync();

            return shoppingCart;


        }
        */

        //Pruebas
        [HttpGet]
        public async Task<IEnumerable<CartContent>> GetShoppingCart(int userId)
        {

            //Recoge el carrito del usuario
            var shoppingCart = await _context.ShoppingCart
            .Where(cart => cart.UserId == userId)  // Filtra por el ID del usuario
            .FirstOrDefaultAsync();

            //Devuelve el contenido del carrito (Productos)
            return await _cartContentRepository.GetByShoppingCartIdAsync(shoppingCart.Id);




        }


        [Authorize]
        [HttpPost]
        public async Task<ShoppingCartDto> CreateOrder([FromBody] ShoppingCartDto orderDto, [FromBody] bool express)
        {
            /* EL FLUJO IRÍA ASÍ:
             * 1) El usuario hace petición post para crear un pedido
             * 2) Si no existen nada en el carro, se crea y se establece ese como el carro 
             * 3) Si el usuario paga, el carro se despeja, en la sección de pagos
             * 4) Si el usuario no paga e intenta crear un nuevo pedido, se agrega el contenido 
             * 5) En caso de que sea un "pago express" (es decir, el usuario se ha metido únicamente para comprar algo), se ignora el paso 4
             * 6) En el front habrá que poner que si ha iniciado sesión solo para pagar, no se llame a la función "GetShoppingCart"
             */
            User user = await GetAuthorizedUser();
            if (user == null)
            {
                return null;
            }
            if (!express)
            {
                // Actualiza el carrito
                Order shoppingCart = ObtainCart(user);
                if (shoppingCart != null)
                {
                    return await UpdateShoppingCart(orderDto, shoppingCart);
                }
            }
            Order order = _orderMapper.ToEntity(orderDto);
            // Fuerzo estos campos para evitar peticiones maliciosas (es decir, que venga Fran a cargarse el back)
            order.UserId = user.Id;
            order.IsReserved = 1;
            order.Payments = null;
            order.CreatedAt = DateTime.Now;
            // AQUÍ NO SE DESCUENTA EL STOCK, ESO SE HARÍA EN EL PAGO
            Order savedOrder = await _unitOfWork.OrderRepository.InsertAsync(order);
            await _unitOfWork.SaveAsync();
            ShoppingCartDto returnedOrder = _orderMapper.ToDto(savedOrder);
            return returnedOrder;
        }



















        private async Task<User> GetAuthorizedUser()
        {
            // Pilla el usuario autenticado según ASP
            System.Security.Claims.ClaimsPrincipal currentUser = this.User;
            string idString = currentUser.Claims.First().ToString().Substring(3); // 3 porque en las propiedades sale "id: X", y la X sale en la tercera posición

            // Pilla el usuario de la base de datos
            return await _unitOfWork.UserRepository.GetAllInfoById(Int32.Parse(idString));
        }

    }
}
