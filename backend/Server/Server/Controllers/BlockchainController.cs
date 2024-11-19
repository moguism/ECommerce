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

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BlockchainController : ControllerBase
{
    private readonly BlockchainService _blockchainService;
    private readonly OrderService _orderService;

    public BlockchainController(BlockchainService blockchainService, OrderService orderService)
    {
        _blockchainService = blockchainService;
        _orderService = orderService;
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

    [HttpPost("check")]
    public async Task<bool> CheckTransactionAsync([FromBody] CheckTransactionRequest data)
    {
        bool done = await _blockchainService.CheckTransactionAsync(data);
        if(done == true)
        {

        }
        return done;
    }
}
