namespace UsersStudentsAPIApp.Repositories
{
    public interface IBaseRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetAsync(int id);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<int> GetCountAsync();
    }
}
