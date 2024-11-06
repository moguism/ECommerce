using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class ProductRepository : Repository<Product, int>
    {

        public ProductRepository(FarminhouseContext context) : base(context) { }

        public PagedDto GetAllProductsByCategory(string productCategory,int pageNumber,int pageSize, IEnumerable<Product> products)
        {
            IEnumerable<Product> productsByCategory = products.Where(product => product.Category.Name.Equals(productCategory));
            int totalProducts = productsByCategory.Count();

            IEnumerable<Product> filteredProducts = productsByCategory.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return new PagedDto
            {
                Products = filteredProducts,
                TotalProducts = totalProducts
            };
        }

        public async Task<Product> GetProductById(int id)
        {
            ICollection<Product> products = await GetAllAsync();

            return products.Where(product => product.Id == id).FirstOrDefault();


        }
    }
}
