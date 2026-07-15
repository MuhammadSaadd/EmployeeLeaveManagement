using EmployeeLeaveManagement.Application.Common;
using EmployeeLeaveManagement.Domain.Common;
using EmployeeLeaveManagement.Domain.Entities;
using EmployeeLeaveManagement.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace EmployeeLeaveManagement.Application.Employees.Commands;

public sealed record CreateEmployeeCommand(
    string EmployeeCode,
    string Name,
    string Department,
    string Email,
    string PhoneNumber) : IRequest<Result<Guid>>;

public sealed class CreateEmployeeCommandValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeCommandValidator()
    {
        RuleFor(x => x.EmployeeCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Department).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(30);
    }
}

public sealed class CreateEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateEmployeeCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var existing = await employeeRepository.GetByEmployeeCodeAsync(request.EmployeeCode, cancellationToken);
        if (existing is not null)
        {
            return Result.Failure<Guid>("An employee with this code already exists.");
        }

        try
        {
            var employee = Employee.Create(
                request.EmployeeCode,
                request.Name,
                request.Department,
                request.Email,
                request.PhoneNumber);

            await employeeRepository.AddAsync(employee, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(employee.Id);
        }
        catch (DomainException ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
    }
}
