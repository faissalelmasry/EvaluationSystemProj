using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.DTOs.Department
{
    public class PagesResult<T>
    {
        public IReadOnlyCollection<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
       
    }
}
