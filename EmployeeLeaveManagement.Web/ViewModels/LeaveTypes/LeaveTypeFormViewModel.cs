using System.ComponentModel.DataAnnotations;

namespace EmployeeLeaveManagement.ViewModels.LeaveTypes;

/// <summary>
/// Create/edit leave type form model.
/// </summary>
public sealed class LeaveTypeFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [Display(Name = "Leave Type Name")]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Maximum Days Allowed")]
    [Range(1, 365)]
    public int MaximumDaysAllowed { get; set; }
}
