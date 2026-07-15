using EmployeeLeaveManagement.Application.Employees.Commands;
using EmployeeLeaveManagement.Application.Employees.Queries;
using EmployeeLeaveManagement.Infrastructure.Identity;
using EmployeeLeaveManagement.ViewModels.Employees;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveManagement.Controllers;

[Authorize(Roles = AppRoles.Manager)]
public sealed class EmployeesController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var employees = await mediator.Send(new GetEmployeesQuery(), cancellationToken);
        return View(employees);
    }

    [HttpGet]
    public IActionResult Create() => View(new EmployeeFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await mediator.Send(new CreateEmployeeCommand(
                model.EmployeeCode,
                model.Name,
                model.Department,
                model.Email,
                model.PhoneNumber), cancellationToken);

            if (result.IsFailure)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return View(model);
            }

            TempData["Success"] = "Employee created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var employee = await mediator.Send(new GetEmployeeByIdQuery(id), cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        return View(new EmployeeFormViewModel
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            Name = employee.Name,
            Department = employee.Department,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EmployeeFormViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await mediator.Send(new UpdateEmployeeCommand(
                id,
                model.EmployeeCode,
                model.Name,
                model.Department,
                model.Email,
                model.PhoneNumber), cancellationToken);

            if (result.IsFailure)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return View(model);
            }

            TempData["Success"] = "Employee updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteEmployeeCommand(id), cancellationToken);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Employee deleted."
            : result.Error;
        return RedirectToAction(nameof(Index));
    }
}
