using AutoMapper;
using EvaluationSystem.Application.DTOs.Department;
using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.Mapping
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<CreateDepartmentDto, Department>();

            CreateMap<Department, DepartmentDto>();
        }
    }
}