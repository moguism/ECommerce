﻿using Examples.WebApi.Models.Dtos;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Server.DTOs;
using Server.Models;
using System.Numerics;

namespace Server.Services.Blockchain;

public class BlockchainService
{
    private readonly UnitOfWork _unitOfWork;

    public BlockchainService(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Erc20ContractDto> GetContractInfoAsync(string nodeUrl, string contractAddress)
    {
        Web3 web3 = new Web3(nodeUrl);
        Contract contract = web3.Eth.GetContract(ERC20ABI, contractAddress);

        string name = await contract.GetFunction("name").CallAsync<string>();
        string symbol = await contract.GetFunction("symbol").CallAsync<string>();
        int decimals = await contract.GetFunction("decimals").CallAsync<int>();
        BigInteger totalSupply = await contract.GetFunction("totalSupply").CallAsync<BigInteger>();

        return new Erc20ContractDto
        {
            Name = name,
            Symbol = symbol,
            Decimals = decimals,
            TotalSupply = totalSupply.ToString()
        };
    }

    public async Task<EthereumTransaction> GetEthereumInfoAsync(CreateTransactionRequest data)
    {
        CoinGeckoApi coinGeckoApi = new CoinGeckoApi();
        EthereumService ethereumService = new EthereumService(data.NetworkUrl);

        decimal ethEurPrice = await coinGeckoApi.GetEthereumPriceAsync();
        BigInteger value = ethereumService.ToWei(data.Euros / ethEurPrice);
        HexBigInteger gas = ethereumService.GetGas();
        HexBigInteger gasPrice = await ethereumService.GetGasPriceAsync();

        EthereumTransaction ethereumTransaction = new EthereumTransaction
        {
            Value = new HexBigInteger(value).HexValue,
            Gas = gas.HexValue,
            GasPrice = gasPrice.HexValue,
        };

        return ethereumTransaction;
    }

    public Task<bool> CheckTransactionAsync(CheckTransactionRequest data)
    {
        EthereumService ethereumService = new EthereumService(data.NetworkUrl);

        return ethereumService.CheckTransactionAsync(data.Hash, data.From, data.To, data.Value);
    }

    public async Task<Order> CreateOrderFromTemporal(string hashOrSessionOrder, string hashOrSessionTemporal, User user, int paymentType)
    {
        Order existingOrder = user.Orders.FirstOrDefault(o => o.HashOrSession.Equals(hashOrSessionOrder));
        if (existingOrder != null)
        {
            return existingOrder;
        }

        //Recoge la ultima orden temporal del usuario
        TemporalOrder temporalOrder = user.TemporalOrders.FirstOrDefault(t => t.HashOrSession.Equals(hashOrSessionTemporal));
        if (temporalOrder == null)
        {
            return null;
        }

        Order order = new Order
        {
            CreatedAt = DateTime.UtcNow,
            PaymentTypeId = paymentType,
            //La misma wishlist que la ultima orden temporal que ha realizado el usuario
            WishlistId = temporalOrder.WishlistId,
            UserId = user.Id,
            HashOrSession = hashOrSessionOrder
        };

        //Elimina el carrito si se ha hecho la compra con sesión iniciada           
        if (!temporalOrder.Quick)
        {
            ShoppingCart shoppingCart = user.ShoppingCart;
            if (shoppingCart != null)
            {
                await _unitOfWork.CartContentRepository.DeleteByIdShoppingCartAsync(shoppingCart);
            }
        }

        //Order en la base de datos
        Order saveOrder = await _unitOfWork.OrderRepository.InsertAsync(order);

        /*saveOrder.Wishlist = temporalOrder.Wishlist;
        _unitOfWork.OrderRepository.Update(saveOrder);

        //Añade la orden a la lista de ordenes del usuario
        user.Orders.Add(saveOrder);
        _unitOfWork.UserRepository.Update(user);*/

        saveOrder.Wishlist = temporalOrder.Wishlist;

        _unitOfWork.TemporalOrderRepository.Delete(temporalOrder);

        await _unitOfWork.SaveAsync();

        return saveOrder;
    }

    public async Task UpdateTemporalOrder(TemporalOrder temporalOrder)
    {
        _unitOfWork.TemporalOrderRepository.Update(temporalOrder);
        await _unitOfWork.SaveAsync();
    }

    public async Task<User> GetUserFromStringWithTemporal(string stringId)
    {
        // Pilla el usuario de la base de datos
        return await _unitOfWork.UserRepository.GetAllInfoWithTemporal(Int32.Parse(stringId));
    }

    // Definición del ABI de ERC-20
    private static readonly string ERC20ABI = """
    [
        {
            'constant': true,
            'inputs': [],
            'name': 'name',
            'outputs': [
                {
                    'name': '',
                    'type': 'string'
                }
            ],
            'payable': false,
            'stateMutability': 'view',
            'type': 'function'
        },
        {
            'constant': true,
            'inputs': [],
            'name': 'symbol',
            'outputs': [
                {
                    'name': '',
                    'type': 'string'
                }
            ],
            'payable': false,
            'stateMutability': 'view',
            'type': 'function'
        },
        {
            'constant': true,
            'inputs': [],
            'name': 'decimals',
            'outputs': [
                {
                    'name': '',
                    'type': 'uint8'
                }
            ],
            'payable': false,
            'stateMutability': 'view',
            'type': 'function'
        },
        {
            'constant': true,
            'inputs': [],
            'name': 'totalSupply',
            'outputs': [
                {
                    'name': '',
                    'type': 'uint256'
                }
            ],
            'payable': false,
            'stateMutability': 'view',
            'type': 'function'
        }
    ]
    """;
}
