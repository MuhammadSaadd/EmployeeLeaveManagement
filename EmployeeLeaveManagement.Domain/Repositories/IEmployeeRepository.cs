using EmployeeLeaveManagement.Domain.Entities;

namespace EmployeeLeaveManagement.Domain.Repositories;

/// <summary>
/// Persistence port for the <see cref="Employee"/> aggregate.
/// </summary>
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Employee?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default);

    Task<Employee?> GetByEmployeeCodeAsync(string employeeCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Employee employee, CancellationToken cancellationToken = default);

    void Update(Employee employee);

    void Remove(Employee employee);

    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
