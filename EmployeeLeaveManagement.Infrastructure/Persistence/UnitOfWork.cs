using EmployeeLeaveManagement.Domain.Repositories;
using EmployeeLeaveManagement.Infrastructure.Persistence;

namespace EmployeeLeaveManagement.Infrastructure.Persistence;

/// <summary>
/// EF Core unit of work implementation.
/// </summary>
public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
