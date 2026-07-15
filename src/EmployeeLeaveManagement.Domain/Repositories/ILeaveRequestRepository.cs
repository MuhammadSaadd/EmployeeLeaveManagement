using EmployeeLeaveManagement.Domain.Entities;
using EmployeeLeaveManagement.Domain.Enums;
using EmployeeLeaveManagement.Domain.ValueObjects;

namespace EmployeeLeaveManagement.Domain.Repositories;

public interface ILeaveRequestRepository
{
    Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveRequest>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingRequestAsync(
        Guid employeeId,
        DateRange dateRange,
        Guid? excludingRequestId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default);

    void Update(LeaveRequest leaveRequest);

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<int> CountByStatusAsync(LeaveRequestStatus status, CancellationToken cancellationToken = default);
}
