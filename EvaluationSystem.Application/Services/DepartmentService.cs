using AutoMapper;
using EvaluationSystem.Application.DTOs.Department;
using EvaluationSystem.Application.Exceptions;
using EvaluationSystem.Application.Helpers;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Domain.Exceptions;
using EvaluationSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EvaluationSystem.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DepartmentService> _logger;
        public DepartmentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<DepartmentService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
        {
            _logger.LogInformation("Creating department: {DepartmentName}", dto.Name);

            await ValidateDepartmentNameAsync(dto.Name);

            var department = _mapper.Map<Department>(dto);

            await _unitOfWork.Departments.AddAsync(department);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Department created successfully: {DepartmentId}", department.Id);
            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<PagesResult<DepartmentDto>> GetAllAsync(DepartmentSort_Page dto)
        {
            var query = _unitOfWork.Departments.GetAll();

            if (!string.IsNullOrWhiteSpace(dto.Search))
            {
                query = query.Where(d => d.Name.Contains(dto.Search) ||
                                         d.Description.Contains(dto.Search));
            }

            var sortOptions = new Dictionary<string, Func<IQueryable<Department>, IQueryable<Department>>>
            {
                ["name"] = q => dto.Descending
                    ? q.OrderByDescending(d => d.Name)
                    : q.OrderBy(d => d.Name),

                ["id"] = q => dto.Descending
                    ? q.OrderByDescending(d => d.Id)
                    : q.OrderBy(d => d.Id),
            };

            query = SortingHelper.ApplySorting(query, dto.SortBy, dto.Descending, sortOptions);

            var totalCount = await query.CountAsync();

            var departments = await query
                .Skip((dto.PageNumber - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToListAsync();

            return new PagesResult<DepartmentDto>
            {
                Items = _mapper.Map<IReadOnlyList<DepartmentDto>>(departments),
                TotalCount = totalCount,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize
            };
        }



        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching department: {DepartmentId}", id);
            var department = await GetDepartmentOrThrowExcpetion(id);

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task UpdateAsync(int id, CreateDepartmentDto dto)
        {
            _logger.LogInformation("Updating department: {DepartmentId}", id);
            var department = await GetDepartmentOrThrowExcpetion(id);

            await ValidateDepartmentNameAsync(dto.Name, id);

            _mapper.Map(dto, department);

            _unitOfWork.Departments.Update(department);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var department = await GetDepartmentOrThrowExcpetion(id); 

           
            var hasUsers = await _unitOfWork.Users
                .FindByCondition(u => u.DepartmentId == id)
                .AnyAsync();

            if (hasUsers)
            {
                throw new BadRequestException("Cannot delete this department because there are users assigned to it. Please reassign them first.");
            }
            department.IsDeleted = true;
            department.DeletedAt = DateTime.UtcNow;

            _unitOfWork.Departments.Update(department);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task ValidateDepartmentNameAsync(string name, int? id = null) { 
            var exists =await _unitOfWork.Departments.FindByCondition(d=>d.Name == name &&
                (id == null || d.Id != id))
                .AnyAsync();
            if (exists)
            {
                throw new BadRequestException("Department Name Already exists");
            }
        }
        public async Task<Department> GetDepartmentOrThrowExcpetion(int id)
        {
            var department=await _unitOfWork.Departments.GetByIdAsync(id);
            return department?? throw new NotFoundException("Department not found") ;
        }
    }
}