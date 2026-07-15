using EmployeeLeaveManagement.Domain.Common;
using EmployeeLeaveManagement.Domain.Enums;
using EmployeeLeaveManagement.Domain.ValueObjects;

namespace EmployeeLeaveManagement.Domain.Entities;

public sealed class LeaveRequest : AggregateRoot
{
    public Guid EmployeeId { get; private set; }

    public Guid LeaveTypeId { get; private set; }

    public DateRange DateRange { get; private set; } = null!;

    public string Reason { get; private set; } = string.Empty;

    public LeaveRequestStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? DecidedAtUtc { get; private set; }

    private LeaveRequest()
    {
    }

    public static LeaveRequest Submit(
        Guid employeeId,
        LeaveType leaveType,
        DateOnly startDate,
        DateOnly endDate,
        string reason,
        DateOnly today,
        bool hasOverlappingRequest)
    {
        ArgumentNullException.ThrowIfNull(leaveType);

        if (employeeId == Guid.Empty)
        {
            throw new DomainException("Employee is required.");
        }

        if (startDate < today)
        {
            throw new DomainException("Start date cannot be earlier than today.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException("Reason is required.");
        }

        if (hasOverlappingRequest)
        {
            throw new DomainException("Employees cannot submit overlapping leave requests.");
        }

        var dateRange = DateRange.Create(startDate, endDate);
        if (dateRange.TotalDays > leaveType.MaximumDaysAllowed)
        {
            throw new DomainException(
                $"Requested days ({dateRange.TotalDays}) exceed the maximum allowed ({leaveType.MaximumDaysAllowed}) for {leaveType.Name}.");
        }

        return new LeaveRequest
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            LeaveTypeId = leaveType.Id,
            DateRange = dateRange,
            Reason = reason.Trim(),
            Status = LeaveRequestStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void Update(
        LeaveType leaveType,
        DateOnly startDate,
        DateOnly endDate,
        string reason,
        DateOnly today,
        bool hasOverlappingRequest)
    {
        EnsureEditable();
        ArgumentNullException.ThrowIfNull(leaveType);

        if (startDate < today)
        {
            throw new DomainException("Start date cannot be earlier than today.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException("Reason is required.");
        }

        if (hasOverlappingRequest)
        {
            throw new DomainException("Employees cannot submit overlapping leave requests.");
        }

        var dateRange = DateRange.Create(startDate, endDate);
        if (dateRange.TotalDays > leaveType.MaximumDaysAllowed)
        {
            throw new DomainException(
                $"Requested days ({dateRange.TotalDays}) exceed the maximum allowed ({leaveType.MaximumDaysAllowed}) for {leaveType.Name}.");
        }

        LeaveTypeId = leaveType.Id;
        DateRange = dateRange;
        Reason = reason.Trim();
    }

    public void Approve()
    {
        EnsurePending();
        Status = LeaveRequestStatus.Approved;
        DecidedAtUtc = DateTime.UtcNow;
    }

    public void Reject()
    {
        EnsurePending();
        Status = LeaveRequestStatus.Rejected;
        DecidedAtUtc = DateTime.UtcNow;
    }

    private void EnsurePending()
    {
        if (Status != LeaveRequestStatus.Pending)
        {
            throw new DomainException("Only pending requests can be approved or rejected.");
        }
    }

    private void EnsureEditable()
    {
        if (Status != LeaveRequestStatus.Pending)
        {
            throw new DomainException("Approved or rejected requests cannot be edited.");
        }
    }
}
