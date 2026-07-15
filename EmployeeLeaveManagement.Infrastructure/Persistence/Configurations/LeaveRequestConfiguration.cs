using EmployeeLeaveManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeLeaveManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF configuration for <see cref="LeaveRequest"/>.
/// </summary>
public sealed class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("LeaveRequests");
        builder.HasKey(lr => lr.Id);

        builder.Property(lr => lr.EmployeeId).IsRequired();
        builder.Property(lr => lr.LeaveTypeId).IsRequired();
        builder.Property(lr => lr.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(lr => lr.Status).IsRequired();
        builder.Property(lr => lr.CreatedAtUtc).IsRequired();

        builder.OwnsOne(lr => lr.DateRange, range =>
        {
            range.Property(r => r.StartDate).HasColumnName("StartDate").IsRequired();
            range.Property(r => r.EndDate).HasColumnName("EndDate").IsRequired();
        });

        builder.HasIndex(lr => lr.EmployeeId);
        builder.HasIndex(lr => lr.Status);
    }
}
