using EmployeeLeaveManagement.Domain.Common;

namespace EmployeeLeaveManagement.Domain.ValueObjects;

public sealed class DateRange : IEquatable<DateRange>
{
    public DateOnly StartDate { get; }

    public DateOnly EndDate { get; }

    private DateRange(DateOnly startDate, DateOnly endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public static DateRange Create(DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate)
        {
            throw new DomainException("End date must be on or after the start date.");
        }

        return new DateRange(startDate, endDate);
    }

    public int TotalDays => EndDate.DayNumber - StartDate.DayNumber + 1;

    public bool Overlaps(DateRange other) =>
        StartDate <= other.EndDate && other.StartDate <= EndDate;

    public bool Equals(DateRange? other) =>
        other is not null && StartDate == other.StartDate && EndDate == other.EndDate;

    public override bool Equals(object? obj) => obj is DateRange other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(StartDate, EndDate);
}
