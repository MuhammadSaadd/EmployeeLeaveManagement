using System.Text.RegularExpressions;
using EmployeeLeaveManagement.Domain.Common;

namespace EmployeeLeaveManagement.Domain.ValueObjects;

public sealed partial class EmailAddress : IEquatable<EmailAddress>
{
    private static readonly Regex EmailRegex = EmailPattern();

    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

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

    public bool Equals(EmailAddress? other) =>
        other is not null && string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(object? obj) => obj is EmailAddress other && Equals(other);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);

    public override string ToString() => Value;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex EmailPattern();
}
