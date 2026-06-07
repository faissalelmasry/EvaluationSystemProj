using EvaluationSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.DTOs.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public JobTitle JobTitle { get; set; }
    }
}
