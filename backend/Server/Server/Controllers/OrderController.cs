using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Mappers;
using Server.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly OrderMapper _orderMapper;

    public OrderController(UnitOfWork unitOfWork, OrderMapper orderMapper)
    {
        _unitOfWork = unitOfWork;
        _orderMapper = orderMapper;
    }

    [Authorize]
    [HttpGet("all")]
    public async Task<IEnumerable<OrderDto>> GetOrders()
    {
        User user = await GetAuthorizedUser();
        if (user == null)
        {
            return null;
        }
        return _orderMapper.ToDto(user.Orders.Where(order => order.IsReserved == 0)); // Los pedidos normales
    }

    [Authorize]
    [HttpPost]
    public async Task<OrderDto> CreateOrder([FromBody] OrderDto orderDto)
    {
        bool express = false;
        /*
         * EL FLUJO IRÍA ASÍ:
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

        if(!express)
        {
            // Actualiza el carrito
            Order shoppingCart = ObtainCart(user);
            if(shoppingCart != null)
            {
                return await UpdateShoppingCart(orderDto, shoppingCart);
            }
        }

        var order = _orderMapper.ToEntity(orderDto);

        // Fuerzo estos campos para evitar peticiones maliciosas (es decir, que venga Fran a cargarse el back)
        order.UserId = user.Id;
        order.IsReserved = 1;
        order.CreatedAt = DateTime.Now;
        order.User = user;

        // AQUÍ NO SE DESCUENTA EL STOCK, ESO SE HARÍA EN EL PAGO

        Order savedOrder = await _unitOfWork.OrderRepository.InsertAsync(order);
        await _unitOfWork.SaveAsync();

        OrderDto returnedOrder = _orderMapper.ToDto(savedOrder);

        return returnedOrder;
    }

    private async Task<OrderDto> UpdateShoppingCart(OrderDto orderDto, Order shoppingCart)
    {
        // DE NUEVO, EN EL PAGO ES DONDE PONDRÍAMOS "IsReserved" A 0

        // Tan solo me interesa actualizar los productos (siento que esto habrá que ampliarlo, idk)
        foreach(Product product in orderDto.Products)
        {
            //shoppingCart.Products.Add(product);
        }

        Order savedOrder = _unitOfWork.OrderRepository.Update(shoppingCart);
        await _unitOfWork.SaveAsync();

        return _orderMapper.ToDto(savedOrder);
    }

    /*
    [Authorize]
    [HttpPut("update-cart")]
    public async Task<OrderDto> UpdateCart([FromBody] OrderDto newCart)
    {
        User user = await GetAuthorizedUser();
        if (user == null)
        {
            return null;
        }

        Order order = user.Orders.Where(getOrder => getOrder.Id == newCart.Id).FirstOrDefault();
        if(order == null)
        {
            return null;
        }

        order.Products = newCart.Products;
        Order savedOrder = _unitOfWork.OrderRepository.Update(order);
        await _unitOfWork.SaveAsync();

        return _orderMapper.ToDto(savedOrder);
    }
    */

    [Authorize]
    [HttpGet("shopping-cart")]
    public async Task<OrderDto> GetShoppingCart()
    {
        User user = await GetAuthorizedUser();
        if (user == null)
        {
            return null;
        }

        Order order = ObtainCart(user);

        if (order == null)
        {
            return null;
        }

        return _orderMapper.ToDto(order); // En teoría solo hay un pedido sin tramitar al mismo tiempo
    }

    private Order ObtainCart(User user)
    {
        IEnumerable<Order> shoppingCart = user.Orders.Where(order => order.IsReserved == 1);

        if (shoppingCart.Count() > 1)
        {
            throw new Exception("ALGUIEN SE HA OLVIDADO DE ACTUALIZAR EN LOS PAGOS");
        }

        return shoppingCart.FirstOrDefault();
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