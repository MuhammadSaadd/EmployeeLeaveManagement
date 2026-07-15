using EmployeeLeaveManagement.Application.Common;
using EmployeeLeaveManagement.Domain.Common;
using EmployeeLeaveManagement.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace EmployeeLeaveManagement.Application.LeaveRequests.Commands;

/// <summary>
/// Approves a pending leave request.
/// </summary>
public sealed record ApproveLeaveRequestCommand(Guid Id) : IRequest<Result>;

/// <summary>
/// Validates <see cref="ApproveLeaveRequestCommand"/>.
/// </summary>
public sealed class ApproveLeaveRequestCommandValidator : AbstractValidator<ApproveLeaveRequestCommand>
{
    public ApproveLeaveRequestCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

/// <summary>
/// Handles <see cref="ApproveLeaveRequestCommand"/>.
/// </summary>
public sealed class ApproveLeaveRequestCommandHandler(
    ILeaveRequestRepository leaveRequestRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ApproveLeaveRequestCommand, Result>
{
    public async Task<Result> Handle(ApproveLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var leaveRequest = await leaveRequestRepository.GetByIdAsync(request.Id, cancellationToken);
        if (leaveRequest is null)
        {
            return Result.Failure("Leave request not found.");
        }

        try
        {
            leaveRequest.Approve();
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
