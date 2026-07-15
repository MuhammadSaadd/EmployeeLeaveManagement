using EmployeeLeaveManagement.Application.Common;
using EmployeeLeaveManagement.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace EmployeeLeaveManagement.Application.LeaveTypes.Commands;

public sealed record DeleteLeaveTypeCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteLeaveTypeCommandValidator : AbstractValidator<DeleteLeaveTypeCommand>
{
    public DeleteLeaveTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class DeleteLeaveTypeCommandHandler(
    ILeaveTypeRepository leaveTypeRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteLeaveTypeCommand, Result>
{
    public async Task<Result> Handle(DeleteLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        var leaveType = await leaveTypeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (leaveType is null)
        {
            return Result.Failure("Leave type not found.");
        }

        leaveTypeRepository.Remove(leaveType);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
