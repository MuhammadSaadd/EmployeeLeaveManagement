using EmployeeLeaveManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeLeaveManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF configuration for <see cref="LeaveType"/>.
/// </summary>
public sealed class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        builder.ToTable("LeaveTypes");
        builder.HasKey(lt => lt.Id);

        builder.Property(lt => lt.Name).HasMaxLength(100).IsRequired();
        builder.Property(lt => lt.MaximumDaysAllowed).IsRequired();
    }
}
