using Server.DTOs;
using Server.Mappers;
using Server.Models;

namespace Server.Services
{
    public class TemporalOrderService
    {

        private readonly UnitOfWork _unitOfWork;
        private readonly CartContentMapper _cartContentMapper;
        private readonly IServiceProvider _serviceProvider;

        public TemporalOrderService(UnitOfWork unitOfWork, CartContentMapper cartContentMapper, IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _cartContentMapper = cartContentMapper;
            _serviceProvider = serviceProvider;
        }

        public async Task<TemporalOrder> GetFullTemporalOrderById(int id)
        {
            return await _unitOfWork.TemporalOrderRepository.GetFullTemporalOrderById(id);
        }

        
        public async Task<TemporalOrder> CreateTemporalOrder(User user, Wishlist wishlist)
        {
            //Crea un nuevo pedido temporal con el usuario y los productos que quiere comprar
            TemporalOrder temporalOrder = new TemporalOrder{
                UserId = user.Id,
                User = user,
                WishlistId = wishlist.Id,
                Wishlist = wishlist
  
            };

            //La añade a la base de datos
            _unitOfWork.TemporalOrderRepository.Add(temporalOrder);

            //Resta el stock a los productos que quiere comprar el usuario
            foreach(ProductsToBuy productToBuy in wishlist.Products)
            {
                Product product = await _unitOfWork.ProductRepository.GetByIdAsync(productToBuy.ProductId);
                product.Stock -= productToBuy.Quantity;
                _unitOfWork.ProductRepository.Update(product);
            }

            await _unitOfWork.SaveAsync();
            return temporalOrder;
        }
        


        /*public async Task RemoveExpiredOrders()
        {
            List<TemporalOrder> expiredOrders = (List<TemporalOrder>) await _unitOfWork.TemporalOrderRepository.GetExpiredOrders(DateTime.UtcNow);

            foreach (TemporalOrder temporalOrder in expiredOrders)
            {
                _unitOfWork.TemporalOrderRepository.Delete(temporalOrder);
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateExpiration(TemporalOrder temporalOrder)
        {
            temporalOrder.ExpirationDate = DateTime.UtcNow;
            using (var scope = _serviceProvider.CreateScope()) // Crea la instancia del serviceProvider
            {
                var unitOfWork = scope.ServiceProvider.GetService<UnitOfWork>();

                unitOfWork.TemporalOrderRepository.Update(temporalOrder);

                await unitOfWork.SaveAsync();
            } // Cierra la instancia del serviceProvider
        }
        */
    }
}
