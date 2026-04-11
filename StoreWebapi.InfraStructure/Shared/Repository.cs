using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using StoreWebapi.Domain.Interfaces;
using StoreWebapi.Infrastructure.Data;

namespace StoreWebapi.Infrastructure.Shared;

public class Repository :IRepository
{
    private readonly AppDbContext _context;
    public Repository(AppDbContext context)
    {
        _context = context;
    }

    #region query

    public async Task<T> FindById<T>(Guid id) where T : class
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public async Task<List<T>> FindAll<T>() where T : class
    {
        return await _context.Set<T>().ToListAsync();
    }

    public async Task<List<T>> FindAll<T>(Expression<Func<T, bool>> predicate) where T : class
    {
        return await  _context.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate) where T : class
    {
       var x = await  _context.Set<T>().Where(predicate).AnyAsync();
       return x;
    }

    #endregion
    
    
    #region command

    public void Add<T>(T item) where T : class => _context.Set<T>().Add(item);

    public void Update<T>(T item) where T : class => _context.Set<T>().Update(item);

    public void Remove<T>(T item) where T : class => _context.Set<T>().Remove(item);

    
    public async Task<int> SaveChangesAsync(CancellationToken ct = default) 
        => await _context.SaveChangesAsync(ct);
    #endregion
}