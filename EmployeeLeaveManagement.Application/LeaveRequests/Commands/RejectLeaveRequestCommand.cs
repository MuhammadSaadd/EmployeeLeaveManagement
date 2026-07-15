using EmployeeLeaveManagement.Application.Common;
using EmployeeLeaveManagement.Domain.Common;
using EmployeeLeaveManagement.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace EmployeeLeaveManagement.Application.LeaveRequests.Commands;

/// <summary>
/// Rejects a pending leave request.
/// </summary>
public sealed record RejectLeaveRequestCommand(Guid Id) : IRequest<Result>;

/// <summary>
/// Validates <see cref="RejectLeaveRequestCommand"/>.
/// </summary>
public sealed class RejectLeaveRequestCommandValidator : AbstractValidator<RejectLeaveRequestCommand>
{
    public RejectLeaveRequestCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

/// <summary>
/// Handles <see cref="RejectLeaveRequestCommand"/>.
/// </summary>
public sealed class RejectLeaveRequestCommandHandler(
    ILeaveRequestRepository leaveRequestRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RejectLeaveRequestCommand, Result>
{
    public async Task<Result> Handle(RejectLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leaveRequest = await leaveRequestRepository.GetByIdAsync(request.Id, cancellationToken);
        if (leaveRequest is null)
        {
            return Result.Failure("Leave request not found.");
        }

        try
        {
            leaveRequest.Reject();
            leaveRequestRepository.Update(leaveRequest);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
