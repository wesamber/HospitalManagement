using HospitalManagement.Domain.Contracts;

namespace HospitalManagement.Application.Interfaces.Repositories;

public interface IRepository<T> where T : IEntity
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
