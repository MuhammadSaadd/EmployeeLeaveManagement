using EmployeeLeaveManagement.Domain.Common;
using EmployeeLeaveManagement.Domain.Enums;
using EmployeeLeaveManagement.Domain.ValueObjects;

namespace EmployeeLeaveManagement.Domain.Entities;

/// <summary>
/// Leave request aggregate capturing an employee's leave application and decision.
/// </summary>
public sealed class LeaveRequest : AggregateRoot
{
    /// <summary>
    /// Gets the requesting employee id.
    /// </summary>
    public Guid EmployeeId { get; private set; }

    /// <summary>
    /// Gets the leave type id.
    /// </summary>
    public Guid LeaveTypeId { get; private set; }

    /// <summary>
    /// Gets the leave date range.
    /// </summary>
    public DateRange DateRange { get; private set; } = null!;

    /// <summary>
    /// Gets the reason for leave.
    /// </summary>
    public string Reason { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the current request status.
    /// </summary>
    public LeaveRequestStatus Status { get; private set; }

    /// <summary>
    /// Gets when the request was submitted (UTC).
    /// </summary>
    public DateTime CreatedAtUtc { get; private set; }

    /// <summary>
    /// Gets when the request was decided (UTC), if any.
    /// </summary>
    public DateTime? DecidedAtUtc { get; private set; }

    private LeaveRequest()
    {
    }

    /// <summary>
    /// Submits a new leave request enforcing date and allowance rules.
    /// </summary>
    /// <param name="employeeId">Requesting employee.</param>
    /// <param name="leaveType">Leave type defining max days.</param>
    /// <param name="startDate">Start date.</param>
    /// <param name="endDate">End date.</param>
    /// <param name="reason">Reason text.</param>
    /// <param name="today">Today's date used to validate start date.</param>
    /// <param name="hasOverlappingRequest">Whether an overlapping active request exists.</param>
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

    /// <summary>
    /// Updates a pending leave request. Approved or rejected requests cannot be edited.
    /// </summary>
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

    /// <summary>
    /// Approves a pending leave request.
    /// </summary>
    public void Approve()
    {
        EnsurePending();
        Status = LeaveRequestStatus.Approved;
        DecidedAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Rejects a pending leave request.
    /// </summary>
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
