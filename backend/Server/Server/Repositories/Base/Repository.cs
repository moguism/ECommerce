using Microsoft.EntityFrameworkCore;

namespace Server.Repositories.Base;

// TODO: Completar

public abstract class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class
{
    protected readonly FarminhouseContext _context;

    public Repository(FarminhouseContext context)
    {
        _context = context;
    }

    public async Task<ICollection<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToArrayAsync();
    }

    public async Task<TEntity> GetByIdAsync(TId id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public IQueryable<TEntity> GetQueryable(bool asNoTracking = true)
    {
        DbSet<TEntity> entities = _context.Set<TEntity>();
        return asNoTracking ? entities.AsNoTracking() : entities; // "AsNoTracking" permite optimizar la consulta para que no se traten los datos
    }

    public Task<TEntity> DeleteAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistAsync(TId id)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> InsertAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveAsync()
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> UpdateAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }
}

