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


    public async ShoppingCartDto ToDto(ShoppingCart cart)
    {
        IEnumerable<CartContent> cartContent = await _cartContentRepository.GetByShoppingCartIdAsync(cart.Id);
        IEnumerable<Product> products = await _productRepository.GetByIdAsync(cart.Id);

        return new ShoppingCartDto
        {
            Id = cart.Id,
            Products = ,
        };
    }

    public IEnumerable<ShoppingCartDto> ToDto(IEnumerable<Order> orders)
    {
        return orders.Select(ToDto);
    }

    public Order ToEntity(ShoppingCartDto orderDto)
    {
        return new Order
        {
            Id = orderDto.Id,
            Products = orderDto.Products
        };
    }

    public IEnumerable<Order> ToEntity(IEnumerable<ShoppingCartDto> ordersDto)
    {
        return ordersDto.Select(ToEntity);
    }
    
}
