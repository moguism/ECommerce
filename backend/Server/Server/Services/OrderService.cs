using Server.Models;
using Stripe.Checkout;

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

            /*if (user.TemporalOrders.Count() == 0)
            {
                throw new Exception("ALGUIEN LA HA LIADO CON LAS ORDENES TEMPORALES");
            }*/

            //Recoge la ultima orden temporal del usuario
            TemporalOrder temporalOrder = await _unitOfWork.TemporalOrderRepository.GetFullTemporalOrderByUserId(user.Id);

            /*if(temporalOrder == null)
            {
                temporalOrder = user.TemporalOrders.LastOrDefault();
            }*/
            
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

            return saveOrder;
        }

        public async Task<Order> CompleteEthTransaction(string hash, User user)
        {
            Order existingOrder = await _unitOfWork.OrderRepository.GetByHash(hash);
            if (existingOrder != null)
            {
                return existingOrder;
            }

            /*if (user.TemporalOrders.Count() == 0)
            {
                throw new Exception("ALGUIEN LA HA LIADO CON LAS ORDENES TEMPORALES");
            }*/

            //Recoge la ultima orden temporal del usuario
            TemporalOrder temporalOrder = await _unitOfWork.TemporalOrderRepository.GetFullTemporalOrderByUserId(user.Id);

            /*if (temporalOrder == null)
            {
                temporalOrder = user.TemporalOrders.LastOrDefault();
            }*/

            Order order = new Order();
            order.CreatedAt = DateTime.UtcNow;
            //order.Total = temporalOrder.Wishlist.Products.Sum(p => p.Product.Price * p.Quantity);

            order.PaymentTypeId = 2;
            //order.PaymentsType = await _unitOfWork.PaymentsTypeRepository.GetByIdAsync(1);

            //La misma wishlist que la ultima orden temporal que ha realizado el usuario
            order.WishlistId = temporalOrder.WishlistId;
            //order.Wishlist = temporalOrder.Wishlist;
            order.UserId = user.Id;
            order.Hash = hash;

            if (!temporalOrder.Quick)
            {
                ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetIdShoppingCartByUserId(user.Id);
                await _unitOfWork.CartContentRepository.DeleteByIdShoppingCartAsync(shoppingCart);
            }

            Order saveOrder = await _unitOfWork.OrderRepository.InsertAsync(order);

            await _unitOfWork.SaveAsync();

            //Añade la orden a la lista de ordenes del usuario
            user.Orders.Add(saveOrder);
            _unitOfWork.UserRepository.Update(user);

            return saveOrder;
        }




        public async Task<IEnumerable<Order>> GetAllOrders(User user)
        {
            return await _unitOfWork.OrderRepository.GetAllOrdersByUserId(user.Id);

        }

        public async Task<Order> GetOrderById(int orderId)
        {
            return await _unitOfWork.OrderRepository.GetById(orderId);

        }




    }
}
