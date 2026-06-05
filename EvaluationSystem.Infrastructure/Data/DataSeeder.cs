using EvaluationSystem.Infrastructure.Data;
using EvaluationSystem.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EvaluationSystem.Infrastructure.Seeds
{
    public static class DataSeeder
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<User>>();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<Role>>();

            // ======================
            // Roles
            // ======================
            string[] roles =
            {
                "Admin",
                "Evaluator",
                "Evaluatee",
                "Reviewer"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new Role
                    {
                        Name = role
                    });
                }
            }

            // ======================
            // Departments
            // ======================
            if (!await context.Departments.AnyAsync())
            {
                context.Departments.AddRange(
                    new Department
                    {
                        Name = "Human Resources",
                        Description = "HR Department"
                    },
                    new Department
                    {
                        Name = "Engineering",
                        Description = "IT & Development"
                    },
                    new Department
                    {
                        Name = "Marketing",
                        Description = "Marketing & Sales"
                    },
                    new Department
                    {
                        Name = "Finance",
                        Description = "Finance Department"
                    }
                );

                await context.SaveChangesAsync();
            }

            // ======================
            // Admin User
            // ======================
            if (await userManager.FindByEmailAsync("admin@evaluation.com") == null)
            {
                var admin = new User
                {
                    UserName = "admin@evaluation.com",
                    Email = "admin@evaluation.com",
                    FullName = "Adham Abdellatif",
                    DepartmentId = 1,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // ======================
            // Evaluator User
            // ======================
            if (await userManager.FindByEmailAsync("manager@evaluation.com") == null)
            {
                var evaluator = new User
                {
                    UserName = "manager@evaluation.com",
                    Email = "manager@evaluation.com",
                    FullName = "Faissal Mahmoud",
                    DepartmentId = 2,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(evaluator, "Manager@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(evaluator, "Evaluator");
                }
            }

            // ======================
            // Evaluatee User
            // ======================
            if (await userManager.FindByEmailAsync("employee@evaluation.com") == null)
            {
                var employee = new User
                {
                    UserName = "employee@evaluation.com",
                    Email = "employee@evaluation.com",
                    FullName = "Aly Gamal",
                    DepartmentId = 2,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(employee, "Employee@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(employee, "Evaluatee");
                }
            }
        }
    }
}