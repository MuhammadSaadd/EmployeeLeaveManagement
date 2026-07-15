using EmployeeLeaveManagement.Application.Common;
using EmployeeLeaveManagement.Domain.Common;
using EmployeeLeaveManagement.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace EmployeeLeaveManagement.Application.Employees.Commands;

/// <summary>
/// Updates an existing employee.
/// </summary>
public sealed record UpdateEmployeeCommand(
    Guid Id,
    string EmployeeCode,
    string Name,
    string Department,
    string Email,
    string PhoneNumber) : IRequest<Result>;

/// <summary>
/// Validates <see cref="UpdateEmployeeCommand"/>.
/// </summary>
public sealed class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.EmployeeCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Department).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(30);
    }
}

/// <summary>
/// Handles <see cref="UpdateEmployeeCommand"/>.
/// </summary>
public sealed class UpdateEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateEmployeeCommand, Result>
{
    public async Task<Result> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (employee is null)
        {
            return Result.Failure("Employee not found.");
        }

        var existing = await employeeRepository.GetByEmployeeCodeAsync(request.EmployeeCode, cancellationToken);
        if (existing is not null && existing.Id != request.Id)
        {
            return Result.Failure("An employee with this code already exists.");
        }

        try
        {
            employee.UpdateDetails(
                request.EmployeeCode,
                request.Name,
                request.Department,
                request.Email,
                request.PhoneNumber);

            employeeRepository.Update(employee);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
