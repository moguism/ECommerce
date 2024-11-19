using Server.DTOs;
using Server.Models;

namespace Server.Mappers
{
    public class OrderMapper
    {

        public OrderDto ToDto(Order order, IEnumerable<CartContentDto> products)
        {


            return new OrderDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt,
                PaymentTypeId = order.PaymentTypeId,
                UserId = order.UserId,
                Products = products
            };
        }
    }
}
