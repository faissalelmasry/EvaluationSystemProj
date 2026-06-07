using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaluationSystem.Application.DTOs.Assignments;

namespace EvaluationSystem.Application.interfaces
{
    public interface IEvaluationAssignmentService
    {
        Task<AssignmentResponseDto> CreateAssignmentAsync(CreateAssignmentDto dto, int adminId);
        Task<IEnumerable<AssignmentResponseDto>> GetAllAssignmentsAsync();
        Task<IEnumerable<AssignmentResponseDto>> GetMyPendingEvaluationsAsync(int evaluatorId);
    }
}

