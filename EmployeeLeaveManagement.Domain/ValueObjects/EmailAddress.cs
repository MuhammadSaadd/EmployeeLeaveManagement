using System.Text.RegularExpressions;
using EmployeeLeaveManagement.Domain.Common;

namespace EmployeeLeaveManagement.Domain.ValueObjects;

/// <summary>
/// Immutable email address value object with format validation.
/// </summary>
public sealed partial class EmailAddress : IEquatable<EmailAddress>
{
    private static readonly Regex EmailRegex = EmailPattern();

    /// <summary>
    /// Gets the normalized email value.
    /// </summary>
    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a validated email address.
    /// </summary>
    /// <param name="value">Raw email string.</param>
    /// <returns>A valid <see cref="EmailAddress"/>.</returns>
    /// <exception cref="DomainException">Thrown when the value is missing or invalid.</exception>
    public static EmailAddress Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Email is required.");
        }

        var normalized = value.Trim();
        if (!EmailRegex.IsMatch(normalized))
        {
            throw new DomainException("Email address is not valid.");
        }

        return new EmailAddress(normalized);
    }

    /// <inheritdoc />
    public bool Equals(EmailAddress? other) =>
        other is not null && string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is EmailAddress other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    /// <inheritdoc />
    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailPattern();
}
