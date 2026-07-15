using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeLeaveManagement.ViewModels.LeaveRequests;

/// <summary>
/// Apply-for-leave form model.
/// </summary>
public sealed class ApplyLeaveViewModel
{
    [Required]
    [Display(Name = "Leave Type")]
    public Guid LeaveTypeId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Start Date")]
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "End Date")]
    public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required]
    [StringLength(1000)]
    public string Reason { get; set; } = string.Empty;

    public IEnumerable<SelectListItem> LeaveTypes { get; set; } = [];
}
