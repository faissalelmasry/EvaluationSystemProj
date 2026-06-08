using EvaluationSystem.Domain.Enums;

namespace EvaluationSystem.Application.DTOs.User
{
    public class UpdateUserDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int DepartmentId { get; set; }
        public JobTitle JobTitle { get; set; }
    }
}