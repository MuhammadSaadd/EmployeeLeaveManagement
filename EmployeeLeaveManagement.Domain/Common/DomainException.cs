namespace EmployeeLeaveManagement.Domain.Common;

/// <summary>
/// Thrown when a domain invariant or business rule is violated.
/// </summary>
public sealed class DomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException"/> class.
    /// </summary>
    /// <param name="message">Explanation of the rule violation.</param>
    public DomainException(string message)
        : base(message)
    {
    }
}
