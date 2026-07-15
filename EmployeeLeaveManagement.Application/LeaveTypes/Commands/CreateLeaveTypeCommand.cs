using EmployeeLeaveManagement.Application.Common;
using EmployeeLeaveManagement.Domain.Common;
using EmployeeLeaveManagement.Domain.Entities;
using EmployeeLeaveManagement.Domain.Repositories;
using FluentValidation;
using MediatR;

namespace EmployeeLeaveManagement.Application.LeaveTypes.Commands;

public sealed record CreateLeaveTypeCommand(string Name, int MaximumDaysAllowed) : IRequest<Result<Guid>>;

public sealed class CreateLeaveTypeCommandValidator : AbstractValidator<CreateLeaveTypeCommand>
{
    public CreateLeaveTypeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MaximumDaysAllowed).GreaterThan(0);
    }
}

public sealed class CreateLeaveTypeCommandHandler(
    ILeaveTypeRepository leaveTypeRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateLeaveTypeCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateLeaveTypeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var leaveType = LeaveType.Create(request.Name, request.MaximumDaysAllowed);
            await leaveTypeRepository.AddAsync(leaveType, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(leaveType.Id);
        }
        catch (DomainException ex)
        {
            return Result.Failure<Guid>(ex.Message);
        }
    }
}
