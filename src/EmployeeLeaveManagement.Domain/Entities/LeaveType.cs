using EmployeeLeaveManagement.Domain.Common;

namespace EmployeeLeaveManagement.Domain.Entities;

public sealed class LeaveType : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;

    public int MaximumDaysAllowed { get; private set; }

    private LeaveType()
    {
    }

    public static LeaveType Create(string name, int maximumDaysAllowed)
    {
        var leaveType = new LeaveType { Id = Guid.NewGuid() };
        leaveType.Update(name, maximumDaysAllowed);
        return leaveType;
    }

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
