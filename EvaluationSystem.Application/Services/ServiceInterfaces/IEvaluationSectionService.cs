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
        Task AddSectionAsync(int templateId, AddEvaluationSectionDto dto);
        Task UpdateSectionAsync(int id, AddEvaluationSectionDto dto);
        Task DeleteSectionAsync(int id);

    }
}
