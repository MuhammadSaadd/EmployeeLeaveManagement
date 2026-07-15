using EmployeeLeaveManagement.Application.Common;
using EmployeeLeaveManagement.Domain.Common;
using EmployeeLeaveManagement.Domain.Entities;
using EmployeeLeaveManagement.Domain.Repositories;
using EmployeeLeaveManagement.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace EmployeeLeaveManagement.Application.LeaveRequests.Commands;

public sealed record SubmitLeaveRequestCommand(
    Guid EmployeeId,
    Guid LeaveTypeId,
    DateOnly StartDate,
    DateOnly EndDate,
    string Reason) : IRequest<Result<Guid>>;

public sealed class SubmitLeaveRequestCommandValidator : AbstractValidator<SubmitLeaveRequestCommand>
{
    public SubmitLeaveRequestCommandValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.LeaveTypeId).NotEmpty();
        RuleFor(x => x.StartDate).NotEmpty();
        RuleFor(x => x.EndDate).NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be on or after the start date.");
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(1000);
    }
}

public sealed class SubmitLeaveRequestCommandHandler(
    IEmployeeRepository employeeRepository,
    ILeaveTypeRepository leaveTypeRepository,
    ILeaveRequestRepository leaveRequestRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<SubmitLeaveRequestCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(SubmitLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
        {
            return Result.Failure<Guid>("Employee not found.");
        }

        var leaveType = await leaveTypeRepository.GetByIdAsync(request.LeaveTypeId, cancellationToken);
        if (leaveType is null)
        {
            return Result.Failure<Guid>("Leave type not found.");
        }

        DateRange dateRange;
        try
        {
            dateRange = DateRange.Create(request.StartDate, request.EndDate);
        }
        catch (DomainException ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }

        var hasOverlap = await leaveRequestRepository.HasOverlappingRequestAsync(
            request.EmployeeId,
            dateRange,
            excludingRequestId: null,
            cancellationToken);

        try
        {
            var leaveRequest = LeaveRequest.Submit(
                request.EmployeeId,
                leaveType,
                request.StartDate,
                request.EndDate,
                request.Reason,
                DateOnly.FromDateTime(DateTime.UtcNow),
                hasOverlap);

            await leaveRequestRepository.AddAsync(leaveRequest, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(leaveRequest.Id);
        }
        catch (DomainException ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
    }
}
