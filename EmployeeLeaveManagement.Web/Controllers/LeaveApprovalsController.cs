using EmployeeLeaveManagement.Application.LeaveRequests.Commands;
using EmployeeLeaveManagement.Application.LeaveRequests.Queries;
using EmployeeLeaveManagement.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveManagement.Controllers;

[Authorize(Roles = AppRoles.Manager)]
public sealed class LeaveApprovalsController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var requests = await mediator.Send(new GetLeaveRequestsQuery(), cancellationToken);
        return View(requests);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ApproveLeaveRequestCommand(id), cancellationToken);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Leave request approved."
            : result.Error;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RejectLeaveRequestCommand(id), cancellationToken);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Leave request rejected."
            : result.Error;
        return RedirectToAction(nameof(Index));
    }
}
