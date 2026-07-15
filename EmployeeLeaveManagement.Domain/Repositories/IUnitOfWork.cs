namespace EmployeeLeaveManagement.Domain.Repositories;

/// <summary>
/// Unit of work that commits aggregate changes.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Persists pending changes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of state entries written.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
