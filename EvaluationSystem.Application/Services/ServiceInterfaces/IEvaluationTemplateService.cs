using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.Services.ServiceInterfaces
{
    public interface IEvaluationTemplateService
    {
        Task<List<EvaluationTemplate>> GetTemplatesAsync(int PageNumber = 1,int PageSize = 10,string? Search = "");
    }
}
