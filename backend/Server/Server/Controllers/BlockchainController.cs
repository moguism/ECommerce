using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Services;
using System.Numerics;
using Server.Models;
using Server.Services.Blockchain;
using Nethereum.Hex.HexTypes;
using System.Web;
using Examples.WebApi.Models.Dtos;
using Server.Mappers;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlockchainController : ControllerBase
{
    private readonly BlockchainService _blockchainService;
    private readonly OrderService _orderService;
    private readonly UserService _userService;
    private readonly ProductsToBuyMapper _productsToBuyMapper;
    private readonly OrderMapper _orderMapper;
    private readonly EmailService _emailService;
    private readonly WishListService _wishListService;
    private readonly TemporalOrderService _temporalOrderService;

    public BlockchainController(BlockchainService blockchainService, OrderService orderService, 
        UserService userService, ProductsToBuyMapper productsToBuyMapper, OrderMapper orderMapper, 
        EmailService emailService, WishListService wishListService, TemporalOrderService temporalOrderService)
    {
        _blockchainService = blockchainService;
        _orderService = orderService;
        _userService = userService;
        _productsToBuyMapper = productsToBuyMapper;
        _orderMapper = orderMapper;
        _emailService = emailService;
        _wishListService = wishListService;
        _temporalOrderService = temporalOrderService;
    }

    [HttpGet]
    public Task<Erc20ContractDto> GetContractInfoAsync([FromQuery] ContractInfoRequest data)
    {
        return _blockchainService.GetContractInfoAsync(data.NetworkUrl, data.ContractAddress);
    }

    // OBTIENE LOS DATOS DE LA TRANSACCIÓN
    [Authorize]
    [HttpPost("transaction")]
    public async Task<EthereumTransaction> CreateTransaction([FromBody] CreateTransactionRequest data)
    {
        User user = await GetAuthorizedUser();
        if(user == null)
        {
            return null;
        }

        data.NetworkUrl = HttpUtility.UrlDecode(data.NetworkUrl);

        EthereumTransaction ethereumTransaction = await _blockchainService.GetEthereumInfoAsync(data);

        TemporalOrder temporalOrder = await _temporalOrderService.GetFullTemporalOrderByUserId(user.Id);
        if (temporalOrder == null)
        {
            return null;
        }
        Wishlist wishlist = await _wishListService.GetWishlistByIdAsync(temporalOrder.WishlistId);
        if (wishlist == null)
        {
            return null;
        }

        decimal total = wishlist.Products.Sum(product => product.PurchasePrice / 100m);

        if(data.Euros >= total)
        {
            temporalOrder.HashOrSession = ethereumTransaction.Value;
            await _temporalOrderService.UpdateTemporalOrder(temporalOrder);
        }
        else
        {
            return null;
        }

        return ethereumTransaction;
    }

    [Authorize]
    [HttpPost("check")]
    public async Task<Order> CheckTransactionAsync([FromBody] CheckTransactionRequest data)
    {
        User user = await GetAuthorizedUser();
        if(user == null)
        {
            return null;
        }

        bool done = await _blockchainService.CheckTransactionAsync(data);
        if(done == true)
        {
            Order order = await _orderService.CompleteEthTransaction(data, user);

            if(order == null)
            {
                return null;
            }

            int wishlistId = order.WishlistId;
            Wishlist productsorder = await _wishListService.GetWishlistByIdAsync(wishlistId);
            await _emailService.CreateEmailUser(user, productsorder, order.PaymentTypeId);

            order.Wishlist = productsorder;
            order.User = null;

            //Productos comprados por el usuario
            //IEnumerable<CartContentDto> products = _productsToBuyMapper.ToDto(order.Wishlist.Products);

            /*Order completedOrder = await _orderService.GetOrderById(order.Id);
            return completedOrder;>*/

            Order returnOrder = await _orderService.GetOrder(order);
            returnOrder.User = null;

            return returnOrder;
        }
        return null;
    }

    private async Task<User> GetAuthorizedUser()
    {
        // Pilla el usuario autenticado según ASP
        System.Security.Claims.ClaimsPrincipal currentUser = this.User;
        string idString = currentUser.Claims.First().ToString().Substring(3); // 3 porque en las propiedades sale "id: X", y la X sale en la tercera posición

        // Pilla el usuario de la base de datos
        return await _userService.GetUserFromDbByStringId(idString);
    }
}
