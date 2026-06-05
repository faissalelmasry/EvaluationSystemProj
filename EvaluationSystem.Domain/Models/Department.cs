using EvaluationSystem.Domain.BaseModels;

namespace EvaluationSystem.Domain.Models
{
    public class Department: BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
