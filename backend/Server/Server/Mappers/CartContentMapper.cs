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

    public CartContent ToEntity(CartContentDto cartContentDto, ShoppingCart cart)
    {
        return new CartContent
        {
            ProductId = cartContentDto.ProductId,
            Quantity = cartContentDto.Quantity,
            ShoppingCartId = cart.Id
        };
    }

    public IEnumerable<CartContent> ToEntity(IEnumerable<CartContentDto> cartContentDtos, ShoppingCart cart)
    {
        List<CartContent> result = new List<CartContent>();
        foreach(CartContentDto cartContent in cartContentDtos)
        {
            result.Add(ToEntity(cartContent, cart));
        }
        return result;
    }



}
