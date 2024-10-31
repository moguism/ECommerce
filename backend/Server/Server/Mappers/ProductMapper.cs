using Server.Models;

namespace Server.Mappers;

public class ProductMapper
{
    public IEnumerable<Product> AddCorrectPath(IEnumerable<Product> products)
    {
        foreach (Product product in products)
        {
            product.image = "images/" + product.image;
        }
        return products;
    }
}
