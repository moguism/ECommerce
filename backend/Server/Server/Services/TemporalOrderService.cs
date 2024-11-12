using Server.DTOs;
using Server.Mappers;
using Server.Models;

namespace Server.Services
{
    public class TemporalOrderService
    {

        private readonly UnitOfWork _unitOfWork;
        private readonly CartContentMapper _cartContentMapper;

        public TemporalOrderService(UnitOfWork unitOfWork, CartContentMapper cartContentMapper)
        {
            _unitOfWork = unitOfWork;
            _cartContentMapper = cartContentMapper;
        }

        public async Task<TemporalOrder> GetFullTemporalOrderById(int id)
        {
            return await _unitOfWork.TemporalOrderRepository.GetFullTemporalOrderById(id);
        }

        public async Task<TemporalOrder> CreateTemporalOrder(TemporalOrder temporalOrder, User user)
        {
            TemporalOrder savedTemporalOrder = await _unitOfWork.TemporalOrderRepository.InsertAsync(temporalOrder);

            ShoppingCart shoppingCart = await _unitOfWork.ShoppingCartRepository.GetAllByUserIdAsync(user.Id, true);
            List<CartContent> cartContents = (List<CartContent>)shoppingCart.CartContent;
            foreach(CartContent cartContent in cartContents)
            {
                Product product = await _unitOfWork.ProductRepository.GetByIdAsync(cartContent.ProductId);
                product.Stock -= cartContent.Quantity;
                _unitOfWork.ProductRepository.Update(product);
            }

            await _unitOfWork.SaveAsync();
            return savedTemporalOrder;
        }

        public async Task AddDirectTemporalOrder(IEnumerable<CartContentDto> cartContentsDto, ShoppingCart cart)
        {
            IEnumerable<CartContent> cartContents = _cartContentMapper.ToEntity(cartContentsDto, cart);
            foreach (CartContent cartContent in cartContents)
            {
                await _unitOfWork.CartContentRepository.InsertAsync(cartContent);
            }
            await _unitOfWork.SaveAsync();
        }
    }
}
