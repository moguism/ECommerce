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

    public IEnumerable<CartContent> ToEntity(IEnumerable<CartContentDto> cartContentDtos)
    {
        List<CartContent> result = new List<CartContent>();
        foreach(CartContentDto cartContent in cartContentDtos)
        {
            result.Add(ToEntity(cartContent));
        }
        return result;
    }



}
