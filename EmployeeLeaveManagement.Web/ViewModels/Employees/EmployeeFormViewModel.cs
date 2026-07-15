using System.ComponentModel.DataAnnotations;

namespace EmployeeLeaveManagement.ViewModels.Employees;

/// <summary>
/// Create/edit employee form model.
/// </summary>
public sealed class EmployeeFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [Display(Name = "Employee Code")]
    [StringLength(50)]
    public string EmployeeCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Employee Name")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Phone Number")]
    [StringLength(30)]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
}
