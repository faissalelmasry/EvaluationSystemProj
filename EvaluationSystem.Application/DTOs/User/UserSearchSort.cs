using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.DTOs.User
{
    public class UserSearchSort:Params
    {
        public string? Search { get; set; }
        public string? Email { get; set; }
    }
}
