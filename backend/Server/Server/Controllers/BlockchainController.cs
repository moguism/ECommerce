﻿using Microsoft.AspNetCore.Authorization;
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
    private readonly EmailService _emailService;
    private readonly TemporalOrderService _temporalOrderService;

    public BlockchainController(BlockchainService blockchainService, EmailService emailService, TemporalOrderService temporalOrderService)
    {
        _blockchainService = blockchainService;
        _emailService = emailService;
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
        User user = await GetMinimumUser();
        if (user == null)
        {
            return null;
        }

        data.NetworkUrl = HttpUtility.UrlDecode(data.NetworkUrl);

        EthereumTransaction ethereumTransaction = await _blockchainService.GetEthereumInfoAsync(data);

        TemporalOrder temporalOrder = await _temporalOrderService.GetLastTemporalOrder(user.Id);
        if (temporalOrder == null)
        {
            return null;
        }

        decimal total = temporalOrder.Wishlist.Products.Sum(product => product.PurchasePrice / 100m);

        if (data.Euros >= total)
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
        User user = await GetAuthorizedUserWithCart();
        if (user == null)
        {
            return null;
        }

        bool done = await _blockchainService.CheckTransactionAsync(data);
        if (done == true)
        {
            Order order = await _blockchainService.CreateOrderFromTemporal(data.Hash, data.Value, user, 2);

            if (order == null)
            {
                return null;
            }

            await _emailService.CreateEmailUser(user, order.Wishlist, order.PaymentTypeId);
            order.User = null;

            return order;
        }
        return null;
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