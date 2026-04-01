using System.Linq.Expressions;

namespace StoreWebapi.Domain.Interfaces;

public interface IRepository
{
    
    public Task<T> FindById<T>(Guid id) where T : class;
    public Task<List<T>> FindAll<T>() where T : class;
    public Task<List<T>> FindAll<T>(Expression<Func<T, bool>>predicate) where T : class;

    #region commands

    void Add<T>(T item) where T : class;
    void Update<T>(T item) where T : class;
    void Remove<T>(T item) where T : class;
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    #endregion
}