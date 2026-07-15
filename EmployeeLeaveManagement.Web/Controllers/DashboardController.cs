using EmployeeLeaveManagement.Application.Dashboard.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveManagement.Controllers;

/// <summary>
/// Displays leave management dashboard metrics.
/// </summary>
[Authorize]
public sealed class DashboardController(IMediator mediator) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var stats = await mediator.Send(new GetDashboardStatsQuery(), cancellationToken);
        return View(stats);
    }
}
