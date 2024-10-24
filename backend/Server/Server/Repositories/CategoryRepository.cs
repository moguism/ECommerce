using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories;

public class CategoryRepository : Repository<Category, int>
{
    public CategoryRepository(FarminhouseContext context): base(context) { }

    public async Task<ICollection<Category>> GetAllWithFullDataAsync()
    {
        return await GetQueryable()
            .Include(category => category.Products)
            .ToArrayAsync();
    }

    public async Task<Category> GetById(int id)
    {
        // "FirstOrDefaultAsync" DEVUELVE NULO SI NO EXISTE
        Category category = await GetQueryable()
            .Include(category => category.Products)
            .FirstOrDefaultAsync(category => category.Id == id);
        return category;
    }
}
