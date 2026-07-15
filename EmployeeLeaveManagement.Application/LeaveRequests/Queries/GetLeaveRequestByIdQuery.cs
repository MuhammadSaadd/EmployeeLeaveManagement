using EmployeeLeaveManagement.Application.LeaveRequests.Models;
using EmployeeLeaveManagement.Domain.Repositories;
using MediatR;

namespace EmployeeLeaveManagement.Application.LeaveRequests.Queries;

/// <summary>
/// Returns a single leave request by id.
/// </summary>
public sealed record GetLeaveRequestByIdQuery(Guid Id) : IRequest<LeaveRequestDto?>;

/// <summary>
/// Handles <see cref="GetLeaveRequestByIdQuery"/>.
/// </summary>
public sealed class GetLeaveRequestByIdQueryHandler(
    ILeaveRequestRepository leaveRequestRepository,
    IEmployeeRepository employeeRepository,
    ILeaveTypeRepository leaveTypeRepository)
    : IRequestHandler<GetLeaveRequestByIdQuery, LeaveRequestDto?>
{
    public async Task<LeaveRequestDto?> Handle(GetLeaveRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var leaveRequest = await leaveRequestRepository.GetByIdAsync(request.Id, cancellationToken);
        if (leaveRequest is null)
        {
            return null;
        }

        var employee = await employeeRepository.GetByIdAsync(leaveRequest.EmployeeId, cancellationToken);
        var leaveType = await leaveTypeRepository.GetByIdAsync(leaveRequest.LeaveTypeId, cancellationToken);

        return new LeaveRequestDto
        {
            Id = leaveRequest.Id,
            EmployeeId = leaveRequest.EmployeeId,
            EmployeeName = employee?.Name ?? "Unknown",
            EmployeeCode = employee?.EmployeeCode ?? string.Empty,
            LeaveTypeId = leaveRequest.LeaveTypeId,
            LeaveTypeName = leaveType?.Name ?? "Unknown",
            StartDate = leaveRequest.DateRange.StartDate,
            EndDate = leaveRequest.DateRange.EndDate,
            TotalDays = leaveRequest.DateRange.TotalDays,
            Reason = leaveRequest.Reason,
            Status = leaveRequest.Status,
            CreatedAtUtc = leaveRequest.CreatedAtUtc
        };
    }
}
