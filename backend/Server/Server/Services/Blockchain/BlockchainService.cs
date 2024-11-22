using Examples.WebApi.Models.Dtos;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Server.DTOs;
using System.Numerics;

namespace Server.Services.Blockchain;

public class BlockchainService
{
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

    public decimal ConvertHexToDecimal(string hexValue)
    {
        try
        {
            // Convertir el valor hexadecimal a BigInteger
            BigInteger bigIntValue = BigInteger.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);

            // Dividir el valor por 10^18 para convertir de Wei a Ether
            decimal etherValue = (decimal)bigIntValue / 1000000000000000000m;

            return etherValue;
        }
        catch (FormatException ex)
        {
            // Manejo de error si el formato del valor hexadecimal no es válido
            Console.WriteLine($"Error de formato hexadecimal: {ex.Message}");
            return 0m; // Retornar 0 en caso de error
        }
        catch (OverflowException ex)
        {
            // Manejo de error si el número es demasiado grande para BigInteger
            Console.WriteLine($"Error de desbordamiento: {ex.Message}");
            return 0m; // Retornar 0 en caso de error
        }
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
