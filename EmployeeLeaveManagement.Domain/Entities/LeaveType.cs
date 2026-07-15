using EmployeeLeaveManagement.Domain.Common;

namespace EmployeeLeaveManagement.Domain.Entities;

/// <summary>
/// Leave type aggregate describing an allowance category.
/// </summary>
public sealed class LeaveType : AggregateRoot
{
    /// <summary>
    /// Gets the leave type display name.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the maximum number of days allowed per request.
    /// </summary>
    public int MaximumDaysAllowed { get; private set; }

    private LeaveType()
    {
    }

    /// <summary>
    /// Creates a new leave type.
    /// </summary>
    public static LeaveType Create(string name, int maximumDaysAllowed)
    {
        var leaveType = new LeaveType { Id = Guid.NewGuid() };
        leaveType.Update(name, maximumDaysAllowed);
        return leaveType;
    }

    /// <summary>
    /// Updates leave type details.
    /// </summary>
    public void Update(string name, int maximumDaysAllowed)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Leave type name is required.");
        }

        if (maximumDaysAllowed <= 0)
        {
            throw new DomainException("Maximum days allowed must be greater than zero.");
        }

        Name = name.Trim();
        MaximumDaysAllowed = maximumDaysAllowed;
    }
}
