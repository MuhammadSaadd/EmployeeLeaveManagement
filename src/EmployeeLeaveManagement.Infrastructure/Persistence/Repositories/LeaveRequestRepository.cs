using EmployeeLeaveManagement.Domain.Entities;
using EmployeeLeaveManagement.Domain.Enums;
using EmployeeLeaveManagement.Domain.Repositories;
using EmployeeLeaveManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagement.Infrastructure.Persistence.Repositories;

public sealed class LeaveRequestRepository(ApplicationDbContext context) : ILeaveRequestRepository
{
    public Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.LeaveRequests.FirstOrDefaultAsync(lr => lr.Id == id, cancellationToken);

    public async Task<IReadOnlyList<LeaveRequest>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.LeaveRequests.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LeaveRequest>> GetByEmployeeIdAsync(
        Guid employeeId,
        CancellationToken cancellationToken = default) =>
        await context.LeaveRequests
            .AsNoTracking()
            .Where(lr => lr.EmployeeId == employeeId)
            .ToListAsync(cancellationToken);

    public Task<bool> HasOverlappingRequestAsync(
        Guid employeeId,
        DateRange dateRange,
        Guid? excludingRequestId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.LeaveRequests.Where(lr =>
            lr.EmployeeId == employeeId &&
            lr.Status != LeaveRequestStatus.Rejected &&
            lr.DateRange.StartDate <= dateRange.EndDate &&
            dateRange.StartDate <= lr.DateRange.EndDate);

        if (excludingRequestId.HasValue)
        {
            query = query.Where(lr => lr.Id != excludingRequestId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default) =>
        await context.LeaveRequests.AddAsync(leaveRequest, cancellationToken);

    public void Update(LeaveRequest leaveRequest) => context.LeaveRequests.Update(leaveRequest);

    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        context.LeaveRequests.CountAsync(cancellationToken);

    public Task<int> CountByStatusAsync(LeaveRequestStatus status, CancellationToken cancellationToken = default) =>
        context.LeaveRequests.CountAsync(lr => lr.Status == status, cancellationToken);
}
