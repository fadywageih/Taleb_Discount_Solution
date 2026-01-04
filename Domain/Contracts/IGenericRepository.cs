using Domain.Entities;

namespace Domain.Contracts
{
    public interface IGenericRepository<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {
        Task<TEntity?> GetByIdAsync(Tkey id);
        Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = false);
        Task<TEntity?> GetByIdAsync(Specifications<TEntity> specifications);
        Task<int> CountAsync(Specifications<TEntity> specifications);
        Task<IEnumerable<TEntity>> GetAllAsync(Specifications<TEntity> specifications);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
