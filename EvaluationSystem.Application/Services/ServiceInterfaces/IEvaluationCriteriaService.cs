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
        Task<bool> AddCriteriaAsync(int sectionid, AddEvaluationCriteriaDto dto);
        Task<bool> UpdateCriteriaAsync(int id, AddEvaluationCriteriaDto dto);
        Task<bool> DeleteCriteriaAsync(int id);
    }
}
