using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Application.DTOs.EvaluationTemplate;
using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.Services.ServiceInterfaces
{
    public interface IEvaluationTemplateService
    {
        Task<List<EvaluationTemplateListDto>> GetTemplatesAsync(int PageNumber = 1,int PageSize = 10,string? Search = "");
        Task<GetEvaluationTemplateDto> GetTemplateAsync(int id);
        Task<bool> AddTemplateAsync(EvaluationTemplateDto dto);
        Task<bool> UpdateTemplateAsync(int id,EvaluationTemplateDto dto);
        Task<bool> DeleteTemplateAsync(int id);
    }
}
