using EmployeeLeaveManagement.Domain.Entities;
using EmployeeLeaveManagement.Infrastructure.Identity;
using EmployeeLeaveManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeLeaveManagement.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        foreach (var role in new[] { AppRoles.Manager, AppRoles.Employee })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var manager = await EnsureUserAsync(userManager, "manager@demo.local", "Manager123!", AppRoles.Manager);
        var employeeUser = await EnsureUserAsync(userManager, "employee@demo.local", "Employee123!", AppRoles.Employee);

        if (!await context.LeaveTypes.AnyAsync())
        {
            context.LeaveTypes.AddRange(
                LeaveType.Create("Annual Leave", 20),
                LeaveType.Create("Sick Leave", 10),
                LeaveType.Create("Casual Leave", 5));
            await context.SaveChangesAsync();
        }

        if (!await context.Employees.AnyAsync())
        {
            var managerEmployee = Employee.Create(
                "EMP001",
                "Alex Manager",
                "Human Resources",
                "manager@demo.local",
                "+1-555-0100",
                manager.Id);

            var staffEmployee = Employee.Create(
                "EMP002",
                "Sam Employee",
                "Engineering",
                "employee@demo.local",
                "+1-555-0101",
                employeeUser.Id);

            context.Employees.AddRange(managerEmployee, staffEmployee);
            await context.SaveChangesAsync();
        }
    }

    private static async Task<IdentityUser> EnsureUserAsync(
        UserManager<IdentityUser> userManager,
        string email,
        string password,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Failed to seed user {email}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }

        return user;
    }
}
