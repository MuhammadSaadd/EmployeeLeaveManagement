namespace EmployeeLeaveManagement.Application.Dashboard.Models;

/// <summary>
/// Aggregated counters for the management dashboard.
/// </summary>
public sealed class DashboardStatsDto
{
    public int TotalEmployees { get; init; }
    public int TotalLeaveRequests { get; init; }
    public int PendingRequests { get; init; }
    public int ApprovedRequests { get; init; }
    public int RejectedRequests { get; init; }
}
