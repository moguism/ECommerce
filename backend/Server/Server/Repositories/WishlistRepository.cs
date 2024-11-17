using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories
{
    public class WishlistRepository : Repository<Wishlist, int>
    {
        public WishlistRepository(FarminhouseContext context) : base(context) { }

        public async Task<Wishlist> GetFullByIdAsync (int id)
        {
            return await GetQueryable()
            .Include(wishlist => wishlist.Products)
            .FirstOrDefaultAsync(wishlist => wishlist.Id == id); 
        }
    }
}
