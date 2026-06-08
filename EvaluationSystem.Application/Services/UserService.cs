using AutoMapper;
using EvaluationSystem.Application.DTOs.Department;
using EvaluationSystem.Application.DTOs.User;
using EvaluationSystem.Application.Exceptions;
using EvaluationSystem.Application.Helpers;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Domain.Exceptions;
using EvaluationSystem.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger<UserService> _logger;
        public UserService(IUnitOfWork unitOfWork,IMapper mapper,UserManager<User> userManager,RoleManager<Role> roleManager,ILogger<UserService> logger)
        {
            _unitofwork = unitOfWork;
            _mapper= mapper;
            _userManager= userManager;
            _roleManager= roleManager;
            _logger= logger;
            
        }

        public async Task AssignDepartmentAsync(int userId,AssignDepartmentDto assignDepartment)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new NotFoundException($"User with ID {userId} not found");

            var department = await _unitofwork.Departments.GetByIdAsync(assignDepartment.DepartmentId);
            if (department == null)
                throw new NotFoundException($"Department with ID {assignDepartment.DepartmentId} not found");

            if (user.DepartmentId == assignDepartment.DepartmentId)
                throw new BadRequestException($"User is already assigned to {department.Name}");

            user.DepartmentId =
                assignDepartment.DepartmentId;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new BadRequestException(
                    string.Join(", ",
                    result.Errors.Select(e => e.Description)));
            }
        }
        public async Task AssignRoleAsync(int userId,AssignRoleDto assignRole)
        {
            var user =
                await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var role =
                await _roleManager.FindByNameAsync(
                    assignRole.RoleName);

            if (role == null)
            {
                throw new NotFoundException("Role not found");
            }

            var currentRoles =
                await _userManager.GetRolesAsync(user);

            if (currentRoles.Contains(assignRole.RoleName))
            {
                throw new BadRequestException(
                    "User is already assigned to this role");
            }

            var removeResult =
                await _userManager.RemoveFromRolesAsync(
                    user,
                    currentRoles);

            if (!removeResult.Succeeded)
            {
                throw new BadRequestException(
                    string.Join(", ",
                    removeResult.Errors.Select(e => e.Description)));
            }

            var addResult =
                await _userManager.AddToRoleAsync(
                    user,
                    assignRole.RoleName);

            if (!addResult.Succeeded)
            {
                throw new BadRequestException(
                    string.Join(", ",
                    addResult.Errors.Select(e => e.Description)));
            }
        }

        public async Task<UserDto> CreateAsync(CreateUserDto user)
        {
            _logger.LogInformation("Creating user with email: {Email}", user.Email);
            var existingUser = await _userManager.FindByEmailAsync(user.Email);

            if (existingUser != null)
            {
                _logger.LogWarning("User creation failed - email exists: {Email}", user.Email);
                throw new BadRequestException("This email already exists!");
            }

            var department =
                await _unitofwork.Departments.GetByIdAsync(user.DepartmentId);

            if (department == null)
            {
                throw new NotFoundException("Department not found");
            }

            var roleExists =
                await _roleManager.RoleExistsAsync(user.Role);

            if (!roleExists)
            {
                throw new NotFoundException("Role not found");
            }

            var newUser = _mapper.Map<User>(user);

            var result =
                await _userManager.CreateAsync(
                    newUser,
                    user.Password);

            if (!result.Succeeded)
            {
                throw new BadRequestException(
                    string.Join(", ",
                    result.Errors.Select(e => e.Description)));
            }

            var roleResult =
                await _userManager.AddToRoleAsync(
                    newUser,
                    user.Role);

            if (!roleResult.Succeeded)
            {
                throw new BadRequestException(
                    string.Join(", ",
                    roleResult.Errors.Select(e => e.Description)));
            }
            _logger.LogInformation("User created successfully: {UserId}", newUser.Id);
            return await MapUserToDto(newUser);
        }

  

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userManager.Users
        .Include(u => u.Department) 
        .FirstOrDefaultAsync(u => u.Id == id);


            if (user == null)
            {
                throw new NotFoundException("User not found");
            }


            return await MapUserToDto(user);
        }

        public async Task<PagesResult<UserDto>> ListUsersAsync(UserSearchSort searchSort)
        {
            var query = _unitofwork.Users.GetAll();

            if (!string.IsNullOrWhiteSpace(searchSort.Search))
            {
                query = query.Where(u =>
                    u.FullName.Contains(searchSort.Search) ||
                    u.Email.Contains(searchSort.Search));
            }

            var sortOptions = new Dictionary<string, Func<IQueryable<User>, IQueryable<User>>>
            {
                ["name"] = q => searchSort.Descending
                    ? q.OrderByDescending(u => u.FullName)
                    : q.OrderBy(u => u.FullName),

                ["id"] = q => searchSort.Descending
                    ? q.OrderByDescending(u => u.Id)
                    : q.OrderBy(u => u.Id),
            };

            query = SortingHelper.ApplySorting(
                query,
                searchSort.SortBy,
                searchSort.Descending,
                sortOptions);

            query = query.Include(u => u.Department);

            var totalCount = await query.CountAsync();
            var users = await query
                .Skip((searchSort.PageNumber - 1) * searchSort.PageSize)
                .Take(searchSort.PageSize)
                .ToListAsync();

            // Optimized: Removed .Result inside LINQ to prevent thread starvation/deadlocks
            var userDtos = new List<UserDto>();
            foreach (var u in users)
            {
                var dto = _mapper.Map<UserDto>(u);
                var roles = await _userManager.GetRolesAsync(u);
                dto.Role = roles.FirstOrDefault() ?? string.Empty;
                userDtos.Add(dto);
            }

            return new PagesResult<UserDto>
            {
                Items = userDtos,
                TotalCount = totalCount,
                PageNumber = searchSort.PageNumber,
                PageSize = searchSort.PageSize
            };
        }
        private async Task<UserDto> MapUserToDto(User user)
        {
            var dto = _mapper.Map<UserDto>(user);

            var roles = await _userManager.GetRolesAsync(user);

            dto.Role = roles.FirstOrDefault() ?? string.Empty;

            return dto;
        }
        public async Task SetUserActiveStatusAsync(int userId, bool isActive)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new NotFoundException("User not found");

            if (user.IsActive == isActive)
            {
                string status = isActive ? "activated" : "deactivated";
                throw new BadRequestException($"User is already {status}");
            }

            user.IsActive = isActive;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task ActivateUser(int userId) => await SetUserActiveStatusAsync(userId, true);
        public async Task DeactivateUser(int userId) => await SetUserActiveStatusAsync(userId, false);

        public async Task UpdateAsync(int id, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new NotFoundException("User not found");

            var department = await _unitofwork.Departments.GetByIdAsync(dto.DepartmentId);
            if (department == null)
                throw new NotFoundException("Department not found");

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.UserName = dto.Email; 
            user.DepartmentId = dto.DepartmentId;
            user.JobTitle = dto.JobTitle;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new NotFoundException("User not found");

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}
