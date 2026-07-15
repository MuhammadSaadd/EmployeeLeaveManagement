namespace EmployeeLeaveManagement.Domain.Common;

/// <summary>
/// Base type for domain entities identified by a unique key.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();
}
