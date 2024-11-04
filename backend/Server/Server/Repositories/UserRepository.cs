using Server.Repositories.Base;
using Server.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Server.Repositories
{
    public class UserRepository : Repository<User,int>
    {
        public UserRepository (FarminhouseContext context) : base(context) { }

        
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetAllInfoById(int id)
        {
            return await GetQueryable()
                .Where(user => user.Id == id)
                //.Include(user => user.Orders)
                .Include(user => user.Reviews)
                .FirstOrDefaultAsync();
        }

    }
}
