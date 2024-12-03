﻿using Server.Repositories.Base;
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
            return await GetQueryable()
                .FirstOrDefaultAsync(user => user.Email.Equals(email));
        }

        public async Task<User> GetAllInfoById(int id)
        {
            return await GetQueryable()
                .Include(user => user.ShoppingCart)
                    .ThenInclude(shoppingCart => shoppingCart.CartContent)
                    .ThenInclude(c => c.Product)
                .Include(user => user.Orders)
                    .ThenInclude(order => order.Wishlist)
                    .ThenInclude(wishList => wishList.Products)
                    .ThenInclude(product => product.Product)
                .FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<ICollection<User>> GetAllInfoExceptId(int id)
        {
            return await GetQueryable()
                .Where(user => user.Id != id)
                .ToArrayAsync();
        }

    }
}
