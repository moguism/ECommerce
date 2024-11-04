using Server.DTOs;
using Server.Models;

namespace Server.Mappers;
public class OrderMapper
{
    public OrderDto ToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            Products = order.Products
        };
    }

    public IEnumerable<OrderDto> ToDto(IEnumerable<Order> orders)
    {
        return orders.Select(ToDto);
    }

    public Order ToEntity(OrderDto orderDto)
    {
        return new Order
        {
            Id = orderDto.Id,
            Products = orderDto.Products
        };
    }

    public IEnumerable<Order> ToEntity(IEnumerable<OrderDto> ordersDto)
    {
        return ordersDto.Select(ToEntity);
    }
}
