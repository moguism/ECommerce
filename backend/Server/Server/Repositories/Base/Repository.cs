using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

    public void Delete(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
    }

    public async Task<bool> ExistAsync(TId id)
    {
        return await GetByIdAsync(id) != null;
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        EntityEntry<TEntity> entry = await _context.Set<TEntity>().AddAsync(entity);
        return entry.Entity;
    }

    public TEntity Update(TEntity entity)
    {
        EntityEntry<TEntity> entry = _context.Set<TEntity>().Update(entity);
        return entry.Entity;
    }
}

