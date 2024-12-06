using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories;

public class CategoryRepository : Repository<Category, int>
{
    public CategoryRepository(FarminhouseContext context): base(context) { }
}
