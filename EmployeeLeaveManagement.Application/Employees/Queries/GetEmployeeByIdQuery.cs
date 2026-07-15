using EmployeeLeaveManagement.Application.Employees.Models;
using EmployeeLeaveManagement.Domain.Repositories;
using MediatR;

namespace EmployeeLeaveManagement.Application.Employees.Queries;

/// <summary>
/// Returns a single employee by id.
/// </summary>
public sealed record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeDto?>;

/// <summary>
/// Handles <see cref="GetEmployeeByIdQuery"/>.
/// </summary>
public sealed class GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    public async Task<EmployeeDto?> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await employeeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (employee is null)
        {
            return null;
        }

        return new EmployeeDto
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            Name = employee.Name,
            Department = employee.Department,
            Email = employee.Email.Value,
            PhoneNumber = employee.PhoneNumber
        };
    }
}
