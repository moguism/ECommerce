using Server.DTOs;
using Server.Models;
using Server.Repositories;

namespace Server.Mappers;

public class CartContentMapper
{
    private readonly CartContentRepository _cartContentRepository;
    private readonly ProductRepository _productRepository;


    public CartContentMapper(CartContentRepository cartContentRepository, ProductRepository productRepository)
    {
        _cartContentRepository = cartContentRepository;
        _productRepository = productRepository;
    }

    /*
    public CartContentDto ToDto(CartContent cartContent)
    {
        Product product = _context.Products
            .Where(p => p.Id == cartContent.ProductId)
            .FirstOrDefault();

        return new CartContentDto
        {
            Id = cartContent.ShoppingCartId,
            Quantity = cartContent.Quantity,
            Product = product,
            ShoppingCartId = cartContent.ShoppingCartId
        };
    }



    public async Task<IEnumerable<CartContentDto>> ToDto(ShoppingCart cart)
    {
        ICollection<CartContent> cartContent = await _cartContentRepository.GetAllAsync();

        IEnumerable<CartContentDto> shoppingCartDtos = new List<CartContentDto>();

        foreach (var item in cartContent)
        {
            CartContentDto shoppingCartItem = new CartContentDto
            {
                Id = cart.Id,
                CartContentId = item.Id,
                Product = await _productRepository.GetProductById(item.ProductId),
                Quantity = item.Quantity

            };
        }

        return shoppingCartDtos;

    }

    public CartContent ToEntity(CartContentDto cartContentDto)
    {
        return new CartContent
        {
            Id = cartContentDto.Id,
            Quantity = cartContentDto.Quantity,
            ProductId = cartContentDto.Product.Id,
            ShoppingCartId = cartContentDto.CartContentId
        };
    }

    public async Task<IEnumerable<CartContent>> ToEntity(int shoppingCartId)
    {
        ICollection<CartContent> cartContent = await _cartContentRepository.GetByShoppingCartIdAsync(shoppingCartId);

        return cartContent;

    }
    */
}
