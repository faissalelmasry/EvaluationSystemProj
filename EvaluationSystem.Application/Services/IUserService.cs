using EvaluationSystem.Application.DTOs.Department;
using EvaluationSystem.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Services
{
    public interface IUserService
    {
        Task<UserDto> CreateAsync(CreateUserDto user);
        Task AssignRoleAsync(int id,AssignRoleDto assignRole);
        Task AssignDepartmentAsync(int userId, AssignDepartmentDto assignDepartment);
        Task ActivateUser(int userId);
        Task DeactivateUser(int userId);
        Task<PagesResult<UserDto>> ListUsersAsync(UserSearchSort searchSort);
        Task<UserDto?> GetByIdAsync(int id);

    }
}
