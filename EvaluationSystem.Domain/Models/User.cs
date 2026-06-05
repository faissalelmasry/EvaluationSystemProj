using EvaluationSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace EvaluationSystem.Domain.Models
{
    public class User:IdentityUser<int>
    {
        public string FullName { get; set; }
        public int DepartmentId { get; set; }
        public JobTitle JobTitle { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Department Department { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public ICollection<EvaluationTemplate> CreatedTemplates { get; set; } = new List<EvaluationTemplate>();

    }
}
