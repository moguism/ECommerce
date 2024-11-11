using Server.DTOs;
using Server.Mappers;
using Server.Models;

namespace Server.Services
{
    public class TemporalOrderService
    {

        private readonly UnitOfWork _unitOfWork;

        public TemporalOrderService(UnitOfWork unitOfWork, ShoppingCartMapper shoppingCartMapper)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TemporalOrder> CreateTemporalOrder(TemporalOrder temporalOrder)
        {
            TemporalOrder savedTemporalOrder = await _unitOfWork.TemporalOrderRepository.InsertAsync(temporalOrder);
            await _unitOfWork.SaveAsync();
            return savedTemporalOrder;
        }

    }
}
