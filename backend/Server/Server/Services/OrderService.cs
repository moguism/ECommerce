using Examples.WebApi.Models.Dtos;
using Server.DTOs;
using Server.Models;
using Server.Services.Blockchain;
using Stripe.Checkout;

namespace Server.Services
{
    public class OrderService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly BlockchainService _blockchainService;

        public OrderService(UnitOfWork unitOfWork, BlockchainService blockchainService)
        {
            _unitOfWork = unitOfWork;
            _blockchainService = blockchainService;
        }

        /*public async Task<Order> CompletePayment(Session session)
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

            //Por ahora inserta el pago con tarjeta
            order.PaymentTypeId = 1;

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

            _unitOfWork.TemporalOrderRepository.Delete(temporalOrder);

            await _unitOfWork.SaveAsync();

            return saveOrder;
        }*/

        public async Task<IEnumerable<Order>> GetAllOrders(User user)
        {
            IEnumerable<Order> orders =  await _unitOfWork.OrderRepository.GetAllOrdersByUserId(user.Id);

            List<Order> allUserOrders = new List<Order>();

            foreach (Order order in user.Orders)
            {
                allUserOrders.Add(await GetOrderById(order.Id));
            }

            return allUserOrders;

            
        }

        public async Task<Order> GetOrderById(int orderId)
        {
            Order order =  await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
            order = await GetOrder(order);
            return order;
        }

        public async Task<Order> GetOrder(Order order)
        {
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
