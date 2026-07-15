using EmployeeLeaveManagement.Application.LeaveTypes.Models;
using EmployeeLeaveManagement.Domain.Repositories;
using MediatR;

namespace EmployeeLeaveManagement.Application.LeaveTypes.Queries;

public sealed record GetLeaveTypeByIdQuery(Guid Id) : IRequest<LeaveTypeDto?>;

public sealed class GetLeaveTypeByIdQueryHandler(ILeaveTypeRepository leaveTypeRepository)
    : IRequestHandler<GetLeaveTypeByIdQuery, LeaveTypeDto?>
{
    public async Task<LeaveTypeDto?> Handle(GetLeaveTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var leaveType = await leaveTypeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (leaveType is null)
        {
            return null;
        }

        return new LeaveTypeDto
        {
            Id = leaveType.Id,
            Name = leaveType.Name,
            MaximumDaysAllowed = leaveType.MaximumDaysAllowed
        };
    }
}
