using Server.DTOs;
using Server.Models;

namespace Server.Mappers;

public class TemporalOrderMapper
{
    public TemporalOrderDto ToDto(TemporalOrder temporalOrder)
    {
        return new TemporalOrderDto
        {
            Id = temporalOrder.Id,
            ShoppingCartId = temporalOrder.ShoppingCartId,
            //UserId = temporalOrder.UserId
        };
    }
}
