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

        public async Task<Product> GetFullProductById(int id)
        {
            Product product = await GetQueryable()
            .AsNoTracking()
            .Include(product => product.Category)
            .Include(product => product.Reviews)
            .Include(product => product.CartContents)
            .FirstOrDefaultAsync(p => p.Id == id);
            return product;
        }

        public async Task<IEnumerable<Product>> GetFullProducts()
        {
            return await GetQueryable()
            .Include(product => product.Category)
            .Include(product => product.Reviews)
            .Include(product => product.CartContents)
            .ToArrayAsync();
        }
    }
}
