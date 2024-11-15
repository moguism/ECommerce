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

        /*public async Task CreateOrder(Order order)
        {
            order = await _unitOfWork.OrderRepository.InsertAsync();

        }*/
        public async Task CompletePayment(Session session)
        {
            User user = await _unitOfWork.UserRepository.GetByEmailAsync(session.CustomerEmail);

            ShoppingCart cart = await _shoppingCartService.GetShoppingCartByUserIdAsync(user.Id);

            if (cart.TemporalOrders.Count() == 0 || cart.TemporalOrders.Count() > 1)
            {
                throw new Exception("ALGUIEN LA HA LIADO CON LAS ORDENES TEMPORALES");
            }
            _unitOfWork.ShoppingCartRepository.Update(cart);

            TemporalOrder temporalOrder = cart.TemporalOrders.First();
            temporalOrder.Finished = true;
            temporalOrder.PaymentTypeId = 1; // El pago con tarjeta
            _unitOfWork.TemporalOrderRepository.Update(temporalOrder);

            Order order = new Order();
            order.TemporalOrderId = temporalOrder.Id;
            order.CreatedAt = DateTime.UtcNow;
            // Quizás también deberíamos de guardar el total, pero por ahora no lo hago porque en el front tenemos métodos para eso
            await _unitOfWork.OrderRepository.InsertAsync(order);

            await _unitOfWork.SaveAsync();
        }

    }
}
