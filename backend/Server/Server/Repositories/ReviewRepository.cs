﻿using Microsoft.EntityFrameworkCore;
using Server.Models;
using Server.Repositories.Base;

namespace Server.Repositories;

public class ReviewRepository : Repository<Review, int>
{
    public ReviewRepository(FarminhouseContext context) : base(context) { }

    public async Task<ICollection<Review>> GetAllWithFullDataAsync()
    {
        return await GetQueryable()
            .Include(review => review.Product)
            .Include(review => review.User)
            .ToArrayAsync();
    }

    public async Task<Review> GetById(int id)
    {
        // "FirstOrDefaultAsync" DEVUELVE NULO SI NO EXISTE
        Review review = await GetQueryable()
            .Include(review => review.Product)
            .Include(review => review.User)
            .FirstOrDefaultAsync(review => review.Id == id);
        return review;
    }
}
