using EmployeeLeaveManagement.Domain.Common;
using EmployeeLeaveManagement.Domain.ValueObjects;

namespace EmployeeLeaveManagement.Domain.Entities;

public sealed class Employee : AggregateRoot
{
    public string EmployeeCode { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string Department { get; private set; } = string.Empty;

    public EmailAddress Email { get; private set; } = null!;

    public string PhoneNumber { get; private set; } = string.Empty;

    public string? IdentityUserId { get; private set; }

    private Employee()
    {
    }

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

    public void LinkIdentityUser(string identityUserId)
    {
        if (string.IsNullOrWhiteSpace(identityUserId))
        {
            throw new DomainException("Identity user id is required.");
        }

        IdentityUserId = identityUserId;
    }
}
