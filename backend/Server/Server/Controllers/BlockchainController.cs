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

    public BlockchainController(BlockchainService blockchainService, OrderService orderService, 
        UserService userService, ProductsToBuyMapper productsToBuyMapper, OrderMapper orderMapper, 
        EmailService emailService, WishListService wishListService)
    {
        _blockchainService = blockchainService;
        _orderService = orderService;
        _userService = userService;
        _productsToBuyMapper = productsToBuyMapper;
        _orderMapper = orderMapper;
        _emailService = emailService;
        _wishListService = wishListService;
    }

    [HttpGet]
    public Task<Erc20ContractDto> GetContractInfoAsync([FromQuery] ContractInfoRequest data)
    {
        return _blockchainService.GetContractInfoAsync(data.NetworkUrl, data.ContractAddress);
    }

    // OBTIENE LOS DATOS DE LA TRANSACCIÓN
    [HttpPost("transaction")]
    public Task<EthereumTransaction> CreateTransaction([FromBody] CreateTransactionRequest data)
    {
        data.NetworkUrl = HttpUtility.UrlDecode(data.NetworkUrl);

        return _blockchainService.GetEthereumInfoAsync(data);
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
            Order order = await _orderService.CompleteEthTransaction(data.Hash, user);

            int wishlistId = order.WishlistId;
            Wishlist productsorder = await _wishListService.GetWishlistByIdAsync(wishlistId);
            await _emailService.CreateEmailUser(user, productsorder, order.PaymentTypeId);

            //Productos comprados por el usuario
            //IEnumerable<CartContentDto> products = _productsToBuyMapper.ToDto(order.Wishlist.Products);

            return await _orderService.GetOrderById(order.Id);
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
