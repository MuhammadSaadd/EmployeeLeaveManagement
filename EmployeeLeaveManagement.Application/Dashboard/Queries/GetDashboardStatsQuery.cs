using EmployeeLeaveManagement.Application.Dashboard.Models;
using EmployeeLeaveManagement.Domain.Enums;
using EmployeeLeaveManagement.Domain.Repositories;
using MediatR;

namespace EmployeeLeaveManagement.Application.Dashboard.Queries;

/// <summary>
/// Returns dashboard aggregate statistics.
/// </summary>
public sealed record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;

/// <summary>
/// Handles <see cref="GetDashboardStatsQuery"/>.
/// </summary>
public sealed class GetDashboardStatsQueryHandler(
    IEmployeeRepository employeeRepository,
    ILeaveRequestRepository leaveRequestRepository)
    : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        return new DashboardStatsDto
        {
            TotalEmployees = await employeeRepository.CountAsync(cancellationToken),
            TotalLeaveRequests = await leaveRequestRepository.CountAsync(cancellationToken),
            PendingRequests = await leaveRequestRepository.CountByStatusAsync(LeaveRequestStatus.Pending, cancellationToken),
            ApprovedRequests = await leaveRequestRepository.CountByStatusAsync(LeaveRequestStatus.Approved, cancellationToken),
            RejectedRequests = await leaveRequestRepository.CountByStatusAsync(LeaveRequestStatus.Rejected, cancellationToken)
        };
    }
}
