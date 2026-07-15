using EmployeeLeaveManagement.Application.LeaveRequests.Commands;
using EmployeeLeaveManagement.Application.LeaveRequests.Queries;
using EmployeeLeaveManagement.Application.LeaveTypes.Queries;
using EmployeeLeaveManagement.Domain.Repositories;
using EmployeeLeaveManagement.Infrastructure.Identity;
using EmployeeLeaveManagement.ViewModels.LeaveRequests;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeLeaveManagement.Controllers;

/// <summary>
/// Employee leave request screens.
/// </summary>
[Authorize]
public sealed class LeaveRequestsController(
    IMediator mediator,
    UserManager<IdentityUser> userManager,
    IEmployeeRepository employeeRepository) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        Guid? employeeId = null;
        if (!User.IsInRole(AppRoles.Manager))
        {
            var employee = await GetCurrentEmployeeAsync(cancellationToken);
            if (employee is null)
            {
                TempData["Error"] = "Your account is not linked to an employee record.";
                return View(Array.Empty<Application.LeaveRequests.Models.LeaveRequestDto>());
            }

            employeeId = employee.Id;
        }

        var requests = await mediator.Send(new GetLeaveRequestsQuery(employeeId), cancellationToken);
        return View(requests);
    }

    [HttpGet]
    public async Task<IActionResult> Apply(CancellationToken cancellationToken)
    {
        var model = await BuildApplyModelAsync(new ApplyLeaveViewModel(), cancellationToken);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(ApplyLeaveViewModel model, CancellationToken cancellationToken)
    {
        model = await BuildApplyModelAsync(model, cancellationToken);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var employee = await GetCurrentEmployeeAsync(cancellationToken);
        if (employee is null)
        {
            ModelState.AddModelError(string.Empty, "Your account is not linked to an employee record.");
            return View(model);
        }

        try
        {
            var result = await mediator.Send(new SubmitLeaveRequestCommand(
                employee.Id,
                model.LeaveTypeId,
                model.StartDate,
                model.EndDate,
                model.Reason), cancellationToken);

            if (result.IsFailure)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return View(model);
            }

            TempData["Success"] = "Leave request submitted.";
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

    private async Task<Domain.Entities.Employee?> GetCurrentEmployeeAsync(CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return null;
        }

        return await employeeRepository.GetByIdentityUserIdAsync(user.Id, cancellationToken);
    }

    private async Task<ApplyLeaveViewModel> BuildApplyModelAsync(ApplyLeaveViewModel model, CancellationToken cancellationToken)
    {
        var leaveTypes = await mediator.Send(new GetLeaveTypesQuery(), cancellationToken);
        model.LeaveTypes = leaveTypes.Select(lt => new SelectListItem
        {
            Value = lt.Id.ToString(),
            Text = $"{lt.Name} (max {lt.MaximumDaysAllowed} days)",
            Selected = lt.Id == model.LeaveTypeId
        });
        return model;
    }
}
