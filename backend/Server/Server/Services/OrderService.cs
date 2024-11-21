using Examples.WebApi.Models.Dtos;
using Nethereum.Hex.HexTypes;
using Server.DTOs;
using Server.Models;
using Server.Services.Blockchain;
using Stripe.Checkout;
using System.Numerics;

namespace Server.Services
{
    public class OrderService
    {
        UnitOfWork _unitOfWork;
        /*private readonly ShoppingCartService _shoppingCartService;*/

        public OrderService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            /*_shoppingCartService = shoppingCartService;*/
        }

        public async Task<Order> CompletePayment(Session session)
        {
            Order existingOrder = await _unitOfWork.OrderRepository.GetBySessionId(session.Id);
            if(existingOrder != null)
            {
                return existingOrder;
            }
            User user = await _unitOfWork.UserRepository.GetByEmailAsync(session.CustomerEmail);

            //Recoge la ultima orden temporal del usuario
            TemporalOrder temporalOrder = await _unitOfWork.TemporalOrderRepository.GetFullTemporalOderByHashOrSession(session.Id);

            
            Order order = new Order();
            order.CreatedAt = DateTime.UtcNow;
            //order.Total = temporalOrder.Wishlist.Products.Sum(p => p.Product.Price * p.Quantity);

            //Por ahora inserta el pago con tarjeta
            order.PaymentTypeId = 1;
            //order.PaymentsType = await _unitOfWork.PaymentsTypeRepository.GetByIdAsync(1);

            //La misma wishlist que la ultima orden temporal que ha realizado el usuario
            order.WishlistId = temporalOrder.WishlistId;

            //order.Wishlist = temporalOrder.Wishlist;
            order.UserId = user.Id;
            order.SessionId = session.Id;

            if(!temporalOrder.Quick)
            {
                ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetIdShoppingCartByUserId(user.Id);
                await _unitOfWork.CartContentRepository.DeleteByIdShoppingCartAsync(shoppingCart);
            }

            Order saveOrder = await _unitOfWork.OrderRepository.InsertAsync(order);
            await _unitOfWork.SaveAsync();


            //Añade la orden a la lista de ordenes del usuario
            user.Orders.Add(saveOrder);
            _unitOfWork.UserRepository.Update(user);

            //Añade la misma wishlist que la de la orden temporal
            saveOrder.Wishlist = temporalOrder.Wishlist;
            _unitOfWork.OrderRepository.Update(saveOrder);

            return saveOrder;
        }

        public async Task<Order> CompleteEthTransaction(CheckTransactionRequest data, User user)
        {
            CoinGeckoApi coinGeckoApi = new CoinGeckoApi();
            EthereumService ethereumService = new EthereumService(data.NetworkUrl);
            decimal ethEurPrice = await coinGeckoApi.GetEthereumPriceAsync();

            Order existingOrder = await _unitOfWork.OrderRepository.GetByHash(data.Hash);
            if (existingOrder != null)
            {
                return existingOrder;
            }


            //Recoge la ultima orden temporal del usuario
            TemporalOrder temporalOrder = await _unitOfWork.TemporalOrderRepository.GetFullTemporalOrderByUserId(user.Id);
            if(temporalOrder == null)
            {
                return null;
            }
            Wishlist wishlist = await _unitOfWork.WishlistRepository.GetFullByIdAsync(user.Id);
            if (wishlist == null)
            {
                return null;
            }

            decimal total = wishlist.Products.Sum(product => product.PurchasePrice);

            BigInteger value = ethereumService.ToWei(total / ethEurPrice);
            HexBigInteger gas = ethereumService.GetGas();
            HexBigInteger gasPrice = await ethereumService.GetGasPriceAsync();

            EthereumTransaction ethereumTransaction = new EthereumTransaction
            {
                Value = new HexBigInteger(value).HexValue,
                Gas = gas.HexValue,
                GasPrice = gasPrice.HexValue,
            };

            Order order = new Order {
                CreatedAt = DateTime.UtcNow,
                PaymentTypeId = 2,
                //La misma wishlist que la ultima orden temporal que ha realizado el usuario
                WishlistId = temporalOrder.WishlistId,
                UserId = user.Id,
                Hash = data.Hash
            };


            
            //Elimina el carrito si se ha hecho la compra con sesión iniciada           
            if (!temporalOrder.Quick)
            {
                ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetIdShoppingCartByUserId(user.Id);
                await _unitOfWork.CartContentRepository.DeleteByIdShoppingCartAsync(shoppingCart);
            }

            //Order en la base de datos
            Order saveOrder = await _unitOfWork.OrderRepository.InsertAsync(order);
            await _unitOfWork.SaveAsync();

            saveOrder.Wishlist = temporalOrder.Wishlist;
            //saveOrder.Wishlist = await _unitOfWork.WishlistRepository.GetByIdAsync(saveOrder.WishlistId);
            _unitOfWork.OrderRepository.Update(saveOrder);

            //Añade la orden a la lista de ordenes del usuario
            user.Orders.Add(saveOrder);
            _unitOfWork.UserRepository.Update(user);

            return saveOrder;
        }




        public async Task<IEnumerable<Order>> GetAllOrders(User user)
        {
            IEnumerable<Order> orders =  await _unitOfWork.OrderRepository.GetAllOrdersByUserId(user.Id);

            List<Order> allUserOrders = new List<Order>();

            foreach (Order order in orders)
            {
                allUserOrders.Add(await GetOrderById(order.Id));
            }

            return allUserOrders;

            
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            Order order =  await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            order.Wishlist = await _unitOfWork.WishlistRepository.GetByIdAsync(order.WishlistId);
            order.Wishlist.Products = _unitOfWork.ProductsToBuyRepository
                    .GetAllProductsByWishlistId(order.WishlistId);


            List<ProductsToBuy> products = new List<ProductsToBuy>();

            foreach (var product in order.Wishlist.Products)
            {
                product.Product = await _unitOfWork.ProductRepository.GetFullProductById(product.ProductId);
                products.Add(product);
            }

            order.Wishlist.Products = products;

            return order;
        }


    }
}
