using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Application.DTOs.EvaluationSection;

namespace EvaluationSystem.Application.Services.ServiceInterfaces
{
    public interface IEvaluationSectionService
    {
        Task<bool> AddSectionAsync(int templateId, AddEvaluationSectionDto dto);
        Task<bool> UpdateSectionAsync(int id, AddEvaluationSectionDto dto);
        Task<bool> DeleteSectionAsync(int id);

    }
}
