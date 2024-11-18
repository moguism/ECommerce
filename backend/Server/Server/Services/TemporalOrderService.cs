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

        public async Task<TemporalOrder> GetFullTemporalOrderByUserId(int id)
        {
            return await _unitOfWork.TemporalOrderRepository.GetFullTemporalOrderByUserId(id);
        }

        
        public async Task<TemporalOrder> CreateTemporalOrder(User user, Wishlist wishlist)
        {

            //La añade a la base de datos
            TemporalOrder order = await _unitOfWork.TemporalOrderRepository.InsertAsync(new TemporalOrder
            {
                UserId = user.Id,
                WishlistId = wishlist.Id,
                ExpirationDate = DateTime.UtcNow
            });


            //Resta el stock a los productos que quiere comprar el usuario
            foreach(ProductsToBuy productToBuy in wishlist.Products)
            {
                Product product = await _unitOfWork.ProductRepository.GetByIdAsync(productToBuy.ProductId);
                product.Stock -= productToBuy.Quantity;
                _unitOfWork.ProductRepository.Update(product);
            }

            await _unitOfWork.SaveAsync();
            return order;
        }

        public async Task<TemporalOrder> GetFullTemporalOrderById(int id)
        {
            return await _unitOfWork.TemporalOrderRepository.GetFullTemporalOrderById(id);
        }

        public async Task UpdateExpiration(TemporalOrder temporalOrder)
        {
            temporalOrder.ExpirationDate = DateTime.UtcNow;
            _unitOfWork.TemporalOrderRepository.Update(temporalOrder);
            await _unitOfWork.SaveAsync();
        }
        
    }
}
