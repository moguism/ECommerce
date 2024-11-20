using Server.DTOs;
using Server.Models;

namespace Server.Mappers;

public class ProductMapper
{
    public IEnumerable<Product> AddCorrectPath(IEnumerable<Product> products)
    {
        foreach (Product product in products)
        {
            product.Image = "images/" + product.Image;
        }
        return products;
    }

    public Product AddCorrectPath(Product product)
    {
        product.Image = "images/" + product.Image; 
        return product;
    }

    public ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Average = product.Average,
            Category = product.Category,
            CategoryId = product.CategoryId,
            Description = product.Description,
            Image = product.Image,
            Price = product.Price,
            Reviews = product.Reviews,
            Stock = product.Stock
        };
    }

    public IEnumerable<ProductDto> ToDto(IEnumerable<Product> products)
    {
        return products.Select(ToDto);
    }

    public Product ToEntity(ProductDto productDto)
    {
        return new Product
        {
            Id = productDto.Id,
            Name = productDto.Name,
            Average = productDto.Average,
            Category = productDto.Category,
            CategoryId = productDto.CategoryId,
            Description = productDto.Description,
            Image = productDto.Image,
            Price = productDto.Price,
            Reviews = productDto.Reviews,
            Stock = productDto.Stock
        };
    }

    public Product ToEntity(ProductToInsert productDto)
    {
        return new Product
        {
            Name = productDto.Name,
            CategoryId = productDto.CategoryId,
            Description = productDto.Description,
            Price = productDto.Price,
            Stock = productDto.Stock
        };
    }

    public IEnumerable<Product> ToEntity(IEnumerable<ProductDto> productsDto)
    {
        return productsDto.Select(ToEntity);
    }
}
