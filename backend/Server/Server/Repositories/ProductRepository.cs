using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class ProductRepository : Repository<Product, int>
    {
        public ProductRepository(FarminhouseContext context) : base(context) { }

        public IEnumerable<Product> GetAllProductsByCategory(string productCategory,int pageNumber,int pageSize, IEnumerable<Product> products)
        {
            return products.Where(product => product.Category.Name.Equals(productCategory)).Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
}
