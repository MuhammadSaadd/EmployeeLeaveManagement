namespace EmployeeLeaveManagement.Application.Common;

/// <summary>
/// Simple result wrapper for application operations.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message when the operation failed.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">Error message.</param>
    public static Result Failure(string error) => new(false, error);

    /// <summary>
    /// Creates a successful typed result.
    /// </summary>
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    /// <summary>
    /// Creates a failed typed result.
    /// </summary>
    public static Result<T> Failure<T>(string error) => Result<T>.Failure(error);
}

/// <summary>
/// Typed result wrapper carrying a value on success.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
public sealed class Result<T> : Result
{
    private Result(bool isSuccess, T? value, string? error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the value when the operation succeeded.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Creates a successful typed result.
    /// </summary>
    public static Result<T> Success(T value) => new(true, value, null);

    /// <summary>
    /// Creates a failed typed result.
    /// </summary>
    public new static Result<T> Failure(string error) => new(false, default, error);
}
