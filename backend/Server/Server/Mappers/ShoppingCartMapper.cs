using Microsoft.IdentityModel.Tokens;
using Server.DTOs;
using Server.Models;
using Server.Repositories;

namespace Server.Mappers;
public class ShoppingCartMapper
{

    private readonly FarminhouseContext _context;
    private readonly CartContentRepository _cartContentRepository;
    private readonly ProductRepository _productRepository;

    public ShoppingCartMapper(FarminhouseContext context, CartContent cartContent, CartContentRepository cartContentRepository, ProductRepository productRepository)
    {
        _context = context;
        _cartContentRepository = cartContentRepository;
        _productRepository = productRepository;
    }


    public async Task<IEnumerable<ShoppingCartDto>> ToDto(ShoppingCart cart)
    {
        ICollection<CartContent> cartContent = await _cartContentRepository.GetByShoppingCartIdAsync(cart.Id);

        IEnumerable<ShoppingCartDto> shoppingCartDtos = new List<ShoppingCartDto>();

        foreach (var item in cartContent)
        {
            ShoppingCartDto shoppingCartItem = new ShoppingCartDto
            {
                Id = item.Id,
                Product = await _productRepository.GetProductById(cart.ProductId),
                Quantity = item.Quantity

            };
        }

        return shoppingCartDtos;

    }

    
}
