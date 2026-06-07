using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.DTOs
{
    public class Params
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; }

        public bool Descending { get; set; }
    }
}
