using EmployeeLeaveManagement.Domain.Entities;

namespace EmployeeLeaveManagement.Domain.Repositories;

public interface ILeaveTypeRepository
{
    Task<LeaveType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeaveType>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(LeaveType leaveType, CancellationToken cancellationToken = default);

    void Update(LeaveType leaveType);

    void Remove(LeaveType leaveType);
}
