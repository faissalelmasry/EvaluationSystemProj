using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Application.DTOs.EvaluationCriteria;

namespace EvaluationSystem.Application.Services.ServiceInterfaces
{
    public interface IEvaluationCriteriaService
    {
        Task AddCriteriaAsync(int sectionid, AddEvaluationCriteriaDto dto);
        Task UpdateCriteriaAsync(int id, AddEvaluationCriteriaDto dto);
        Task DeleteCriteriaAsync(int id);
    }
}
