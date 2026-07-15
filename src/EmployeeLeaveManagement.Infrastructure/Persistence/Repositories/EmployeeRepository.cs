using EmployeeLeaveManagement.Domain.Entities;
using EmployeeLeaveManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagement.Infrastructure.Persistence.Repositories;

public sealed class EmployeeRepository(ApplicationDbContext context) : IEmployeeRepository
{
    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<Employee?> GetByIdentityUserIdAsync(string identityUserId, CancellationToken cancellationToken = default) =>
        context.Employees.FirstOrDefaultAsync(e => e.IdentityUserId == identityUserId, cancellationToken);

    public Task<Employee?> GetByEmployeeCodeAsync(string employeeCode, CancellationToken cancellationToken = default) =>
        context.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode, cancellationToken);

    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Employees.AsNoTracking().OrderBy(e => e.Name).ToListAsync(cancellationToken);

    public async Task AddAsync(Employee employee, CancellationToken cancellationToken = default) =>
        await context.Employees.AddAsync(employee, cancellationToken);

    public void Update(Employee employee) => context.Employees.Update(employee);

    public void Remove(Employee employee) => context.Employees.Remove(employee);

    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        context.Employees.CountAsync(cancellationToken);
}
