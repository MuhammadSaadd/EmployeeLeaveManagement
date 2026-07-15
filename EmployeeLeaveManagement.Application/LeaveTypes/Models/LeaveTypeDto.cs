namespace EmployeeLeaveManagement.Application.LeaveTypes.Models;

/// <summary>
/// Read model for leave type screens.
/// </summary>
public sealed class LeaveTypeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int MaximumDaysAllowed { get; init; }
}
