using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class CartContentRepository : Repository<Product, int>
    {

        public CartContentRepository(FarminhouseContext context) : base(context) { }


    }
}
