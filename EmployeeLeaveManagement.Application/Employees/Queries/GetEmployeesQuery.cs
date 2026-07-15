using EmployeeLeaveManagement.Application.Employees.Models;
using EmployeeLeaveManagement.Domain.Repositories;
using MediatR;

namespace EmployeeLeaveManagement.Application.Employees.Queries;

/// <summary>
/// Returns all employees.
/// </summary>
public sealed record GetEmployeesQuery : IRequest<IReadOnlyList<EmployeeDto>>;

/// <summary>
/// Handles <see cref="GetEmployeesQuery"/>.
/// </summary>
public sealed class GetEmployeesQueryHandler(IEmployeeRepository employeeRepository)
    : IRequestHandler<GetEmployeesQuery, IReadOnlyList<EmployeeDto>>
{
    public async Task<IReadOnlyList<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await employeeRepository.GetAllAsync(cancellationToken);
        return employees
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                Name = e.Name,
                Department = e.Department,
                Email = e.Email.Value,
                PhoneNumber = e.PhoneNumber
            })
            .ToList();
    }
}
