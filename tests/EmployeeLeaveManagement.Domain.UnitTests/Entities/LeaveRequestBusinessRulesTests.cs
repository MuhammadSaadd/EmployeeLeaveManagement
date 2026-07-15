using EmployeeLeaveManagement.Domain.Common;
using EmployeeLeaveManagement.Domain.Entities;
using EmployeeLeaveManagement.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace EmployeeLeaveManagement.Domain.UnitTests.Entities;

public sealed class LeaveRequestBusinessRulesTests
{
    private static readonly DateOnly Today = new(2026, 7, 15);
    private static readonly Guid EmployeeId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static LeaveType CreateLeaveType(int maximumDays = 10) =>
        LeaveType.Create("Annual Leave", maximumDays);

    private static LeaveRequest CreatePendingRequest(
        DateOnly? start = null,
        DateOnly? end = null,
        bool hasOverlap = false)
    {
        var startDate = start ?? Today;
        var endDate = end ?? Today.AddDays(2);

        return LeaveRequest.Submit(
            EmployeeId,
            CreateLeaveType(),
            startDate,
            endDate,
            "Family trip",
            Today,
            hasOverlap);
    }

    [Fact]
    public void Submit_WhenStartDateIsToday_Succeeds()
    {
        var act = () => CreatePendingRequest(Today, Today.AddDays(1));

        act.Should().NotThrow();
    }

    [Fact]
    public void Submit_WhenStartDateIsInThePast_Throws()
    {
        var act = () => CreatePendingRequest(Today.AddDays(-1), Today);

        act.Should().Throw<DomainException>()
            .WithMessage("Start date cannot be earlier than today.");
    }

    [Fact]
    public void Submit_WhenEndDateEqualsStartDate_Succeeds()
    {
        var request = CreatePendingRequest(Today, Today);

        request.DateRange.StartDate.Should().Be(Today);
        request.DateRange.EndDate.Should().Be(Today);
        request.Status.Should().Be(LeaveRequestStatus.Pending);
    }

    [Fact]
    public void Submit_WhenEndDateIsBeforeStartDate_Throws()
    {
        var act = () => CreatePendingRequest(Today.AddDays(2), Today);

        act.Should().Throw<DomainException>()
            .WithMessage("End date must be on or after the start date.");
    }

    [Fact]
    public void Submit_WhenOverlappingRequestExists_Throws()
    {
        var act = () => CreatePendingRequest(hasOverlap: true);

        act.Should().Throw<DomainException>()
            .WithMessage("Employees cannot submit overlapping leave requests.");
    }

    [Fact]
    public void Submit_WhenNoOverlappingRequest_Succeeds()
    {
        var request = CreatePendingRequest(hasOverlap: false);

        request.Status.Should().Be(LeaveRequestStatus.Pending);
        request.EmployeeId.Should().Be(EmployeeId);
    }

    [Fact]
    public void Approve_WhenPending_SetsApproved()
    {
        var request = CreatePendingRequest();

        request.Approve();

        request.Status.Should().Be(LeaveRequestStatus.Approved);
        request.DecidedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void Reject_WhenPending_SetsRejected()
    {
        var request = CreatePendingRequest();

        request.Reject();

        request.Status.Should().Be(LeaveRequestStatus.Rejected);
        request.DecidedAtUtc.Should().NotBeNull();
    }

    [Theory]
    [InlineData(nameof(LeaveRequest.Approve))]
    [InlineData(nameof(LeaveRequest.Reject))]
    public void Decide_WhenAlreadyApproved_Throws(string action)
    {
        var request = CreatePendingRequest();
        request.Approve();

        Action act = action switch
        {
            nameof(LeaveRequest.Approve) => request.Approve,
            nameof(LeaveRequest.Reject) => request.Reject,
            _ => throw new ArgumentOutOfRangeException(nameof(action))
        };

        act.Should().Throw<DomainException>()
            .WithMessage("Only pending requests can be approved or rejected.");
    }

    [Theory]
    [InlineData(nameof(LeaveRequest.Approve))]
    [InlineData(nameof(LeaveRequest.Reject))]
    public void Decide_WhenAlreadyRejected_Throws(string action)
    {
        var request = CreatePendingRequest();
        request.Reject();

        Action act = action switch
        {
            nameof(LeaveRequest.Approve) => request.Approve,
            nameof(LeaveRequest.Reject) => request.Reject,
            _ => throw new ArgumentOutOfRangeException(nameof(action))
        };

        act.Should().Throw<DomainException>()
            .WithMessage("Only pending requests can be approved or rejected.");
    }

    [Fact]
    public void Update_WhenPending_Succeeds()
    {
        var request = CreatePendingRequest();
        var leaveType = CreateLeaveType();
        var newStart = Today.AddDays(1);
        var newEnd = Today.AddDays(3);

        request.Update(leaveType, newStart, newEnd, "Updated reason", Today, hasOverlappingRequest: false);

        request.DateRange.StartDate.Should().Be(newStart);
        request.DateRange.EndDate.Should().Be(newEnd);
        request.Reason.Should().Be("Updated reason");
    }

    [Fact]
    public void Update_WhenApproved_Throws()
    {
        var request = CreatePendingRequest();
        request.Approve();

        var act = () => request.Update(
            CreateLeaveType(),
            Today.AddDays(1),
            Today.AddDays(2),
            "Updated reason",
            Today,
            hasOverlappingRequest: false);

        act.Should().Throw<DomainException>()
            .WithMessage("Approved or rejected requests cannot be edited.");
    }

    [Fact]
    public void Update_WhenRejected_Throws()
    {
        var request = CreatePendingRequest();
        request.Reject();

        var act = () => request.Update(
            CreateLeaveType(),
            Today.AddDays(1),
            Today.AddDays(2),
            "Updated reason",
            Today,
            hasOverlappingRequest: false);

        act.Should().Throw<DomainException>()
            .WithMessage("Approved or rejected requests cannot be edited.");
    }

    [Fact]
    public void Update_WhenStartDateIsInThePast_Throws()
    {
        var request = CreatePendingRequest();

        var act = () => request.Update(
            CreateLeaveType(),
            Today.AddDays(-1),
            Today,
            "Updated reason",
            Today,
            hasOverlappingRequest: false);

        act.Should().Throw<DomainException>()
            .WithMessage("Start date cannot be earlier than today.");
    }

    [Fact]
    public void Update_WhenOverlappingRequestExists_Throws()
    {
        var request = CreatePendingRequest();

        var act = () => request.Update(
            CreateLeaveType(),
            Today.AddDays(1),
            Today.AddDays(2),
            "Updated reason",
            Today,
            hasOverlappingRequest: true);

        act.Should().Throw<DomainException>()
            .WithMessage("Employees cannot submit overlapping leave requests.");
    }
}
