using AutoMapper;
using EvaluationSystem.Application.DTOs.Department;
using EvaluationSystem.Application.Exceptions;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Domain.Exceptions;
using EvaluationSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvaluationSystem.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto)
        {
            await ValidateDepartmentNameAsync(dto.Name);

            var department = _mapper.Map<Department>(dto);

            await _unitOfWork.Departments.AddAsync(department);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task<PagesResult<DepartmentDto>> GetAllAsync(DepartmentSort_Page dto)
        {
            var query = _unitOfWork.Departments.GetAll();
            if(!string.IsNullOrWhiteSpace(dto.Search))
            {
                query = query.Where(d => d.Name.Contains(dto.Search) || d.Description.Contains(dto.Search));
            }
            if (!string.IsNullOrWhiteSpace(dto.SortBy))
            {
                switch (dto.SortBy.ToLower())
                {
                    case "name":
                        query = dto.Descending
                            ? query.OrderByDescending(d => d.Name)
                            : query.OrderBy(d => d.Name);
                        break;

                    case "id":
                        query = dto.Descending
                            ? query.OrderByDescending(d => d.Id)
                            : query.OrderBy(d => d.Id);
                        break;

                    default:
                        query = query.OrderBy(d => d.Id);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(d => d.Id);
            }

            var departments = await query
      .Skip((dto.PageNumber - 1) * dto.PageSize)
      .Take(dto.PageSize)
      .ToListAsync();
            var totalCount = await query.CountAsync();
            return new PagesResult<DepartmentDto>
            {
                Items = _mapper.Map<IReadOnlyList<DepartmentDto>>(departments),
                TotalCount =   totalCount,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize
            };
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var department = await GetDepartmentOrThrowExcpetion(id);

            return _mapper.Map<DepartmentDto>(department);
        }

        public async Task UpdateAsync(int id, CreateDepartmentDto dto)
        {
            var department = await GetDepartmentOrThrowExcpetion(id);

            await ValidateDepartmentNameAsync(dto.Name, id);

            _mapper.Map(dto, department);

            _unitOfWork.Departments.Update(department);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
           var department= await GetDepartmentOrThrowExcpetion(id);

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