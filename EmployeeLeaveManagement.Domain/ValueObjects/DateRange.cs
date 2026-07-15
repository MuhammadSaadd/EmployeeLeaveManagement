using EmployeeLeaveManagement.Domain.Common;

namespace EmployeeLeaveManagement.Domain.ValueObjects;

/// <summary>
/// Inclusive date range used by leave requests.
/// </summary>
public sealed class DateRange : IEquatable<DateRange>
{
    /// <summary>
    /// Gets the first day of leave (inclusive).
    /// </summary>
    public DateOnly StartDate { get; }

    /// <summary>
    /// Gets the last day of leave (inclusive).
    /// </summary>
    public DateOnly EndDate { get; }

    private DateRange(DateOnly startDate, DateOnly endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    /// <summary>
    /// Creates a date range ensuring end is on or after start.
    /// </summary>
    /// <param name="startDate">Start date.</param>
    /// <param name="endDate">End date.</param>
    /// <returns>A valid <see cref="DateRange"/>.</returns>
    /// <exception cref="DomainException">Thrown when end precedes start.</exception>
    public static DateRange Create(DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate)
        {
            throw new DomainException("End date must be on or after the start date.");
        }

        return new DateRange(startDate, endDate);
    }

    /// <summary>
    /// Gets the number of calendar days in the range (inclusive).
    /// </summary>
    public int TotalDays => EndDate.DayNumber - StartDate.DayNumber + 1;

    /// <summary>
    /// Determines whether this range overlaps another inclusive range.
    /// </summary>
    /// <param name="other">The other range.</param>
    /// <returns><c>true</c> when the ranges overlap.</returns>
    public bool Overlaps(DateRange other) =>
        StartDate <= other.EndDate && other.StartDate <= EndDate;

    /// <inheritdoc />
    public bool Equals(DateRange? other) =>
        other is not null && StartDate == other.StartDate && EndDate == other.EndDate;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is DateRange other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(StartDate, EndDate);
}
