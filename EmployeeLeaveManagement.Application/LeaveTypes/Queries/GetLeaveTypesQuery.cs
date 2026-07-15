using EmployeeLeaveManagement.Application.LeaveTypes.Models;
using EmployeeLeaveManagement.Domain.Repositories;
using MediatR;

namespace EmployeeLeaveManagement.Application.LeaveTypes.Queries;

public sealed record GetLeaveTypesQuery : IRequest<IReadOnlyList<LeaveTypeDto>>;

public sealed class GetLeaveTypesQueryHandler(ILeaveTypeRepository leaveTypeRepository)
    : IRequestHandler<GetLeaveTypesQuery, IReadOnlyList<LeaveTypeDto>>
{
    public async Task<IReadOnlyList<LeaveTypeDto>> Handle(GetLeaveTypesQuery request, CancellationToken cancellationToken)
    {
        var leaveTypes = await leaveTypeRepository.GetAllAsync(cancellationToken);
        return leaveTypes
            .Select(lt => new LeaveTypeDto
            {
                Id = lt.Id,
                Name = lt.Name,
                MaximumDaysAllowed = lt.MaximumDaysAllowed
            })
            .ToList();
    }
}
