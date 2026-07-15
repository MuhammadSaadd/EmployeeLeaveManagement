using EmployeeLeaveManagement.Application.LeaveRequests.Models;
using EmployeeLeaveManagement.Domain.Repositories;
using MediatR;

namespace EmployeeLeaveManagement.Application.LeaveRequests.Queries;

/// <summary>
/// Returns leave requests, optionally filtered to one employee.
/// </summary>
public sealed record GetLeaveRequestsQuery(Guid? EmployeeId = null) : IRequest<IReadOnlyList<LeaveRequestDto>>;

/// <summary>
/// Handles <see cref="GetLeaveRequestsQuery"/>.
/// </summary>
public sealed class GetLeaveRequestsQueryHandler(
    ILeaveRequestRepository leaveRequestRepository,
    IEmployeeRepository employeeRepository,
    ILeaveTypeRepository leaveTypeRepository)
    : IRequestHandler<GetLeaveRequestsQuery, IReadOnlyList<LeaveRequestDto>>
{
    public async Task<IReadOnlyList<LeaveRequestDto>> Handle(GetLeaveRequestsQuery request, CancellationToken cancellationToken)
    {
        var leaveRequests = request.EmployeeId is null
            ? await leaveRequestRepository.GetAllAsync(cancellationToken)
            : await leaveRequestRepository.GetByEmployeeIdAsync(request.EmployeeId.Value, cancellationToken);

        var employees = (await employeeRepository.GetAllAsync(cancellationToken)).ToDictionary(e => e.Id);
        var leaveTypes = (await leaveTypeRepository.GetAllAsync(cancellationToken)).ToDictionary(lt => lt.Id);

        return leaveRequests
            .OrderByDescending(lr => lr.CreatedAtUtc)
            .Select(lr =>
            {
                employees.TryGetValue(lr.EmployeeId, out var employee);
                leaveTypes.TryGetValue(lr.LeaveTypeId, out var leaveType);

                return new LeaveRequestDto
                {
                    Id = lr.Id,
                    EmployeeId = lr.EmployeeId,
                    EmployeeName = employee?.Name ?? "Unknown",
                    EmployeeCode = employee?.EmployeeCode ?? string.Empty,
                    LeaveTypeId = lr.LeaveTypeId,
                    LeaveTypeName = leaveType?.Name ?? "Unknown",
                    StartDate = lr.DateRange.StartDate,
                    EndDate = lr.DateRange.EndDate,
                    TotalDays = lr.DateRange.TotalDays,
                    Reason = lr.Reason,
                    Status = lr.Status,
                    CreatedAtUtc = lr.CreatedAtUtc
                };
            })
            .ToList();
    }
}
