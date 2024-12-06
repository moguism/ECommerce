using Server.DTOs;
using Server.Models;
using Server.Repositories;

namespace Server.Mappers;

public class CartContentMapper
{   

    public CartContentDto ToDto(CartContent cartContent)
    {
        return new CartContentDto
        {
            ProductId = cartContent.ProductId,
            Quantity = cartContent.Quantity,
        };
    }

    public CartContent ToEntity(CartContentDto cartContentDto)
    {
        return new CartContent
        {
            ProductId = cartContentDto.ProductId,
            Quantity = cartContentDto.Quantity,
        };
    }
}
