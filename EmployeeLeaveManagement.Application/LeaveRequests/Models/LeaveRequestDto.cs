using EmployeeLeaveManagement.Domain.Enums;

namespace EmployeeLeaveManagement.Application.LeaveRequests.Models;

public sealed class LeaveRequestDto
{
    public Guid Id { get; init; }
    public Guid EmployeeId { get; init; }
    public string EmployeeName { get; init; } = string.Empty;
    public string EmployeeCode { get; init; } = string.Empty;
    public Guid LeaveTypeId { get; init; }
    public string LeaveTypeName { get; init; } = string.Empty;
    public DateOnly StartDate { get; init; }
    public DateOnly EndDate { get; init; }
    public int TotalDays { get; init; }
    public string Reason { get; init; } = string.Empty;
    public LeaveRequestStatus Status { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
