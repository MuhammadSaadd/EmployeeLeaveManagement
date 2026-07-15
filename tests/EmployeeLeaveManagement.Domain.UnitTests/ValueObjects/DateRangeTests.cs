using EmployeeLeaveManagement.Domain.Common;
using EmployeeLeaveManagement.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace EmployeeLeaveManagement.Domain.UnitTests.ValueObjects;

public sealed class DateRangeTests
{
    [Fact]
    public void Create_WhenEndEqualsStart_Succeeds()
    {
        var start = new DateOnly(2026, 7, 15);

        var range = DateRange.Create(start, start);

        range.StartDate.Should().Be(start);
        range.EndDate.Should().Be(start);
        range.TotalDays.Should().Be(1);
    }

    [Fact]
    public void Create_WhenEndIsAfterStart_Succeeds()
    {
        var start = new DateOnly(2026, 7, 15);
        var end = new DateOnly(2026, 7, 17);

        var range = DateRange.Create(start, end);

        range.TotalDays.Should().Be(3);
    }

    [Fact]
    public void Create_WhenEndIsBeforeStart_Throws()
    {
        var start = new DateOnly(2026, 7, 17);
        var end = new DateOnly(2026, 7, 15);

        var act = () => DateRange.Create(start, end);

        act.Should().Throw<DomainException>()
            .WithMessage("End date must be on or after the start date.");
    }

    [Theory]
    [InlineData("2026-07-15", "2026-07-20", "2026-07-18", "2026-07-25", true)]
    [InlineData("2026-07-15", "2026-07-20", "2026-07-21", "2026-07-25", false)]
    [InlineData("2026-07-15", "2026-07-20", "2026-07-10", "2026-07-15", true)]
    [InlineData("2026-07-15", "2026-07-20", "2026-07-10", "2026-07-14", false)]
    public void Overlaps_ReturnsExpectedResult(
        string startA,
        string endA,
        string startB,
        string endB,
        bool expected)
    {
        var rangeA = DateRange.Create(DateOnly.Parse(startA), DateOnly.Parse(endA));
        var rangeB = DateRange.Create(DateOnly.Parse(startB), DateOnly.Parse(endB));

        rangeA.Overlaps(rangeB).Should().Be(expected);
    }
}
