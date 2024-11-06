using Server.DTOs;
using Server.Models;
using Server.Repositories;

namespace Server.Mappers;

public class ShoppingCartMapper
{


    public ShoppingCartMapper()
    { 
    }


    public ShoppingCartDto ToDto(ShoppingCart shoppingCart)
    {

        return new ShoppingCartDto
        {
            Id = shoppingCart.Id,
            User = shoppingCart.User,
            CartContent = shoppingCart.CartContent,
        };
    }


    public ShoppingCart ToEntity(ShoppingCartDto shoppingCartDto)
    {
        return new ShoppingCart
        {
            Id = shoppingCartDto.Id,
            UserId = shoppingCartDto.User.Id,
            User = shoppingCartDto.User,
            CartContent = shoppingCartDto.CartContent
        };
    }

   
}
