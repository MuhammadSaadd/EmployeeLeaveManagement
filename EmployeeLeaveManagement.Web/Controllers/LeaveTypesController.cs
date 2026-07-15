using EmployeeLeaveManagement.Application.LeaveTypes.Commands;
using EmployeeLeaveManagement.Application.LeaveTypes.Queries;
using EmployeeLeaveManagement.Infrastructure.Identity;
using EmployeeLeaveManagement.ViewModels.LeaveTypes;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveManagement.Controllers;

/// <summary>
/// Leave type CRUD screens.
/// </summary>
[Authorize(Roles = AppRoles.Manager)]
public sealed class LeaveTypesController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var leaveTypes = await mediator.Send(new GetLeaveTypesQuery(), cancellationToken);
        return View(leaveTypes);
    }

    [HttpGet]
    public IActionResult Create() => View(new LeaveTypeFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LeaveTypeFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var result = await mediator.Send(new CreateLeaveTypeCommand(model.Name, model.MaximumDaysAllowed), cancellationToken);
            if (result.IsFailure)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return View(model);
            }

            TempData["Success"] = "Leave type created successfully.";
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
        var leaveType = await mediator.Send(new GetLeaveTypeByIdQuery(id), cancellationToken);
        if (leaveType is null)
        {
            return NotFound();
        }

        return View(new LeaveTypeFormViewModel
        {
            Id = leaveType.Id,
            Name = leaveType.Name,
            MaximumDaysAllowed = leaveType.MaximumDaysAllowed
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, LeaveTypeFormViewModel model, CancellationToken cancellationToken)
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
            var result = await mediator.Send(new UpdateLeaveTypeCommand(id, model.Name, model.MaximumDaysAllowed), cancellationToken);
            if (result.IsFailure)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return View(model);
            }

            TempData["Success"] = "Leave type updated successfully.";
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
        var result = await mediator.Send(new DeleteLeaveTypeCommand(id), cancellationToken);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Leave type deleted."
            : result.Error;
        return RedirectToAction(nameof(Index));
    }
}
