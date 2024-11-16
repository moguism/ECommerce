using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class WishlistRepository : Repository<Wishlist, int>
    {
        public WishlistRepository(FarminhouseContext context) : base(context) { }


    }
}
