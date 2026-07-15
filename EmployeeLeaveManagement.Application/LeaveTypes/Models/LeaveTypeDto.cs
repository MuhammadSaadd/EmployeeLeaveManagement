namespace EmployeeLeaveManagement.Application.LeaveTypes.Models;

public sealed class LeaveTypeDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int MaximumDaysAllowed { get; init; }
}
