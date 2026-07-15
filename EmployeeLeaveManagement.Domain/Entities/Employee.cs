using EmployeeLeaveManagement.Domain.Common;
using EmployeeLeaveManagement.Domain.ValueObjects;

namespace EmployeeLeaveManagement.Domain.Entities;

/// <summary>
/// Employee aggregate representing a person who can request leave.
/// </summary>
public sealed class Employee : AggregateRoot
{
    /// <summary>
    /// Gets the unique employee code.
    /// </summary>
    public string EmployeeCode { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the employee full name.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the department name.
    /// </summary>
    public string Department { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the employee email.
    /// </summary>
    public EmailAddress Email { get; private set; } = null!;

    /// <summary>
    /// Gets the phone number.
    /// </summary>
    public string PhoneNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the linked ASP.NET Identity user id, when provisioned.
    /// </summary>
    public string? IdentityUserId { get; private set; }

    private Employee()
    {
    }

    /// <summary>
    /// Creates a new employee in a valid state.
    /// </summary>
    public static Employee Create(
        string employeeCode,
        string name,
        string department,
        string email,
        string phoneNumber,
        string? identityUserId = null)
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid()
        };
        employee.UpdateDetails(employeeCode, name, department, email, phoneNumber);
        employee.IdentityUserId = string.IsNullOrWhiteSpace(identityUserId) ? null : identityUserId;
        return employee;
    }

    /// <summary>
    /// Updates mutable employee details.
    /// </summary>
    public void UpdateDetails(
        string employeeCode,
        string name,
        string department,
        string email,
        string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(employeeCode))
        {
            throw new DomainException("Employee code is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Employee name is required.");
        }

        if (string.IsNullOrWhiteSpace(department))
        {
            throw new DomainException("Department is required.");
        }

        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            throw new DomainException("Phone number is required.");
        }

        EmployeeCode = employeeCode.Trim();
        Name = name.Trim();
        Department = department.Trim();
        Email = EmailAddress.Create(email);
        PhoneNumber = phoneNumber.Trim();
    }

    /// <summary>
    /// Associates the employee with an Identity user account.
    /// </summary>
    /// <param name="identityUserId">Identity user id.</param>
    public void LinkIdentityUser(string identityUserId)
    {
        if (string.IsNullOrWhiteSpace(identityUserId))
        {
            throw new DomainException("Identity user id is required.");
        }

        IdentityUserId = identityUserId;
    }
}
