namespace MegaNotes.Api.Interfaces;

public interface IRepository<T, TKey>
{
    IEnumerable<T> GetAll();
    T? GetById(TKey id);
    T Add(T entity);
    T? Update(TKey id, T entity);
    bool Delete(TKey id);
}
