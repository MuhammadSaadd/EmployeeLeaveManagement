using EmployeeLeaveManagement.Application.Common;
using EmployeeLeaveManagement.Domain.Common;
using EmployeeLeaveManagement.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace EmployeeLeaveManagement.Application.LeaveTypes.Commands;

/// <summary>
/// Updates a leave type.
/// </summary>
public sealed record UpdateLeaveTypeCommand(Guid Id, string Name, int MaximumDaysAllowed) : IRequest<Result>;

/// <summary>
/// Validates <see cref="UpdateLeaveTypeCommand"/>.
/// </summary>
public sealed class UpdateLeaveTypeCommandValidator : AbstractValidator<UpdateLeaveTypeCommand>
{
    public UpdateLeaveTypeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MaximumDaysAllowed).GreaterThan(0);
    }
}

/// <summary>
/// Handles <see cref="UpdateLeaveTypeCommand"/>.
/// </summary>
public sealed class UpdateLeaveTypeCommandHandler(
    ILeaveTypeRepository leaveTypeRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateLeaveTypeCommand, Result>
{
    public async Task<Result> Handle(UpdateLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        var leaveType = await leaveTypeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (leaveType is null)
        {
            return Result.Failure("Leave type not found.");
        }

        try
        {
            leaveType.Update(request.Name, request.MaximumDaysAllowed);
            leaveTypeRepository.Update(leaveType);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
