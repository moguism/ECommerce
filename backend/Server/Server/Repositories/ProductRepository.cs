using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Server.DTOs;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class ProductRepository : Repository<Product, int>
    {

        public ProductRepository(FarminhouseContext context) : base(context) { }

        public PagedDto GetAllProductsByCategory(int pageNumber,int pageSize, IEnumerable<Product> products)
        {
            int totalProducts = products.Count();

            IEnumerable<Product> filteredProducts = products.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return new PagedDto
            {
                Products = filteredProducts,
                TotalProducts = totalProducts
            };
        }

        public async Task<Product> GetFullProductById(int id)
        {
            Product product = await GetQueryable()
            .Include(product => product.Category)
            .Include(product => product.Reviews)
            .ThenInclude(review => review.User)
            .FirstOrDefaultAsync(p => p.Id == id);
            return product;
        }
    }
}
