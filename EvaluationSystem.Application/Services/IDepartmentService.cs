using EvaluationSystem.Application.DTOs.Department;
using Microsoft.AspNetCore.Mvc;

public interface IDepartmentService
{
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto dto);

    Task UpdateAsync(int id, CreateDepartmentDto dto);

    Task DeleteAsync(int id);

    Task<PagesResult<DepartmentDto>> GetAllAsync(DepartmentSort_Page query);

    Task<DepartmentDto?> GetByIdAsync(int id);
}