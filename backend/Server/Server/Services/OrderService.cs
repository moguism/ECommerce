using Server.Models;
using Stripe.Checkout;

namespace Server.Services
{
    public class OrderService
    {
        UnitOfWork _unitOfWork;
        private readonly ShoppingCartService _shoppingCartService;

        public OrderService(UnitOfWork unitOfWork, ShoppingCartService shoppingCartService)
        {
            _unitOfWork = unitOfWork;
            _shoppingCartService = shoppingCartService;
        }

        public async Task CompletePayment(Session session)
        {
            User user = await _unitOfWork.UserRepository.GetByEmailAsync(session.CustomerEmail);

            /*if (user.TemporalOrders.Count() == 0)
            {
                throw new Exception("ALGUIEN LA HA LIADO CON LAS ORDENES TEMPORALES");
            }*/

            //Recoge la ultima orden temporal del usuario
            TemporalOrder temporalOrder = await _unitOfWork.TemporalOrderRepository.GetFullTemporalOrderByUserId(user.Id);

            if(temporalOrder == null)
            {
                temporalOrder = user.TemporalOrders.LastOrDefault();
            }

            Order order = new Order();
            order.CreatedAt = DateTime.UtcNow;
            //order.Total = temporalOrder.Wishlist.Products.Sum(p => p.Product.Price * p.Quantity);

            //Por ahora inserta el pago con tarjeta
            order.PaymentTypeId = 1;
            //order.PaymentsType = await _unitOfWork.PaymentsTypeRepository.GetByIdAsync(1);

            //La misma wishlist que la ultima orden temporal que ha realizado el usuario
            order.WishlistId = temporalOrder.WishlistId;
            //order.Wishlist = temporalOrder.Wishlist;


            await _unitOfWork.OrderRepository.InsertAsync(order);

            await _unitOfWork.SaveAsync();
        }

    }
}
