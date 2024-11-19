using Server.DTOs;
using Server.Models;

namespace Server.Mappers;

public class TemporalOrderMapper
{
    ProductsToBuyMapper _productsToBuyMapper;

    public TemporalOrderMapper(ProductsToBuyMapper productsToBuyMapper)
    {
        _productsToBuyMapper = productsToBuyMapper;
    }

    public TemporalOrderDto ToDto(TemporalOrder temporalOrder)
    {
        return new TemporalOrderDto
        {
            Id = temporalOrder.Id,
            UserId = temporalOrder.UserId,
            Quick = temporalOrder.Quick,
            CartContentDtos = _productsToBuyMapper.ToDto(temporalOrder.Wishlist.Products) //Manda al cliente una lista con los productos de la orden temporal para poder mostrarlos
        };
    }
}
