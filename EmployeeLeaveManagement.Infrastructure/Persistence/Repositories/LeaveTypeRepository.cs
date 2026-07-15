using EmployeeLeaveManagement.Domain.Entities;
using EmployeeLeaveManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagement.Infrastructure.Persistence.Repositories;

public sealed class LeaveTypeRepository(ApplicationDbContext context) : ILeaveTypeRepository
{
    public Task<LeaveType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.LeaveTypes.FirstOrDefaultAsync(lt => lt.Id == id, cancellationToken);

    public async Task<IReadOnlyList<LeaveType>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.LeaveTypes.AsNoTracking().OrderBy(lt => lt.Name).ToListAsync(cancellationToken);

    public async Task AddAsync(LeaveType leaveType, CancellationToken cancellationToken = default) =>
        await context.LeaveTypes.AddAsync(leaveType, cancellationToken);

    public void Update(LeaveType leaveType) => context.LeaveTypes.Update(leaveType);

    public void Remove(LeaveType leaveType) => context.LeaveTypes.Remove(leaveType);
}
