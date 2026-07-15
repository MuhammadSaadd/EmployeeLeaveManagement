namespace EmployeeLeaveManagement.Application.Employees.Models;

/// <summary>
/// Read model for employee screens.
/// </summary>
public sealed class EmployeeDto
{
    public Guid Id { get; init; }
    public string EmployeeCode { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}
