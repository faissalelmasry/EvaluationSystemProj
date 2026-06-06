using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.Services.TemplateServices
{
    internal class TemplateService : IEvaluationTemplateService
    {
        public Task<List<EvaluationTemplate>> GetTemplatesAsync(int PageNumber = 1, int PageSize = 10, string? Search = "")
        {
            throw new NotImplementedException();
        }
    }
}
