using EmployeeLeaveManagement.Application.Common;
using EmployeeLeaveManagement.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace EmployeeLeaveManagement.Application.Employees.Commands;

public sealed record DeleteEmployeeCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteEmployeeCommandValidator : AbstractValidator<DeleteEmployeeCommand>
{
    public DeleteEmployeeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class DeleteEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteEmployeeCommand, Result>
{
    public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (employee is null)
        {
            return Result.Failure("Employee not found.");
        }

        employeeRepository.Remove(employee);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
