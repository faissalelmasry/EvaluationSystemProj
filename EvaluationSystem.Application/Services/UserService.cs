using AutoMapper;
using EvaluationSystem.Application.DTOs.Department;
using EvaluationSystem.Application.DTOs.User;
using EvaluationSystem.Application.Exceptions;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Domain.Exceptions;
using EvaluationSystem.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        public UserService(IUnitOfWork unitOfWork,IMapper mapper,UserManager<User> userManager,RoleManager<Role> roleManager)
        {
            _unitofwork = unitOfWork;
            _mapper= mapper;
            _userManager= userManager;
            _roleManager= roleManager;
            
        }

        public async Task ActivateUser(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new BadRequestException("User not found");
            }
            if (user.IsActive)
            {
                throw new BadRequestException("User is already activated");
            }
            user.IsActive = true;
            await _userManager.UpdateAsync(user);
        }

        public async Task AssignDepartmentAsync(int userId,AssignDepartmentDto assignDepartment)
        {
            var user =
                await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var department =
                await _unitofwork.Departments.GetByIdAsync(
                    assignDepartment.DepartmentId);

            if (department == null)
            {
                throw new NotFoundException("Department not found");
            }

            if (user.DepartmentId ==
                assignDepartment.DepartmentId)
            {
                throw new BadRequestException(
                    "User is already assigned to this department");
            }

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
            var existingUser = await _userManager.FindByEmailAsync(user.Email);

            if (existingUser != null)
            {
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

            return await MapUserToDto(newUser);
        }

        public async Task DeactivateUser(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if(user == null)
            {
                throw new BadRequestException("User not found");
            }
            if (!user.IsActive)
            {
                throw new BadRequestException("User is already deactivated");
            }
            user.IsActive = false;
            await _userManager.UpdateAsync(user);
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _userManager.Users
        .Include(u => u.Department) 
        .FirstOrDefaultAsync(u => u.Id.ToString() == id.ToString());


            if (user == null)
            {
                throw new NotFoundException("User not found");
            }


            return await MapUserToDto(user);
        }

        public async Task<PagesResult<UserDto>> ListUsersAsync( UserSearchSort searchSort)
        {
            searchSort.PageNumber =
                Math.Max(1, searchSort.PageNumber);

            searchSort.PageSize =
                Math.Clamp(searchSort.PageSize, 1, 100);

            var query = _unitofwork.Users.GetAll();

            query = query.Include(u => u.Department);

            if (!string.IsNullOrWhiteSpace(searchSort.Search))
            {
                query = query.Where(u =>
                    u.FullName.Contains(searchSort.Search) ||
                    u.Email.Contains(searchSort.Search));
            }

            query = searchSort.SortBy?.ToLower() switch
            {
                "name" => searchSort.Descending
                    ? query.OrderByDescending(u => u.FullName)
                    : query.OrderBy(u => u.FullName),

                "id" => searchSort.Descending
                    ? query.OrderByDescending(u => u.Id)
                    : query.OrderBy(u => u.Id),

                _ => query.OrderBy(u => u.Id)
            };

            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((searchSort.PageNumber - 1) * searchSort.PageSize)
                .Take(searchSort.PageSize)
                .ToListAsync();

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                userDtos.Add(await MapUserToDto(user));
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
    }
}
