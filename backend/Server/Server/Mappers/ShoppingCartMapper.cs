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
        ShoppingCartDto cartDto = new ShoppingCartDto
        {
            Id = shoppingCart.Id,
            UserId = shoppingCart.User.Id,
            CartContent = shoppingCart.CartContent,
        };

        return cartDto;
    }   
}
