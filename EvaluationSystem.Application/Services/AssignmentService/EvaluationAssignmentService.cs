using AutoMapper;
using EvaluationSystem.Application.DTOs.Assignments;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Domain.Enums;
using EvaluationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Services.AssignmentService
{
    public class EvaluationAssignmentService : IEvaluationAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepo<User> _userRepo;
        private readonly IMapper _mapper;
        private readonly IGenericRepo<EvaluationTemplate> _evalutionTemplate;

        public EvaluationAssignmentService(IUnitOfWork unitOfWork, IGenericRepo<User> userRepo, IMapper mapper, IGenericRepo<EvaluationTemplate> evalutionTemplate)
        {
            _unitOfWork = unitOfWork;
            _userRepo = userRepo;
            _mapper = mapper;
            _evalutionTemplate = evalutionTemplate;
        }

        public async Task<AssignmentResponseDto> CreateAssignmentAsync(CreateAssignmentDto dto, int adminId)
        {
            if (dto.EvaluatorId == dto.EvaluateeId)
            {
                throw new Exception("The evaluator cannot be the same person as the evaluatee.");
            }

            if (dto.DueDate <= DateTime.UtcNow)
            {
                throw new Exception("The evaluation due date must be in the future.");
            }

            var admin = await _userRepo.GetByIdAsync(adminId);
            if (admin == null)
            {
                throw new Exception("The operating admin was not found in the system.");
            }

            if (admin.JobTitle != JobTitle.Manager)
            {
                throw new Exception("The provided Admin ID does not have the authority to assign evaluations.");
            }

            var template = await _evalutionTemplate.GetByIdAsync(dto.TemplateId);
            if (template == null)
            {
                throw new Exception("The specified evaluation template was not found.");
            }

            var evaluator = await _userRepo.GetByIdAsync(dto.EvaluatorId);
            var evaluatee = await _userRepo.GetByIdAsync(dto.EvaluateeId);

            if (evaluator == null || evaluatee == null)
            {
                throw new Exception("The evaluator or evaluatee was not found in the system.");
            }

            if (evaluator.JobTitle == JobTitle.Student && evaluatee.JobTitle == JobTitle.Manager)
            {
                throw new Exception("A student is not allowed to evaluate a manager.");
            }

            if (evaluator.JobTitle == JobTitle.Employee && evaluatee.JobTitle == JobTitle.Manager)
            {
                throw new Exception("An employee cannot evaluate a manager through this assignment pathway.");
            }

            var assignment = new EvaluationAssignment
            {
                TemplateId = dto.TemplateId,
                EvaluatorId = dto.EvaluatorId,
                EvaluateeId = dto.EvaluateeId,
                AssignedById = adminId,
                Status = EvaluationStatus.Pending,
                DueDate = dto.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.EvaluationAssignments.AddAsync(assignment);
            await _unitOfWork.SaveChangesAsync();

            var createdAssignment = await _unitOfWork.EvaluationAssignments
                .FindByCondition(a => a.Id == assignment.Id, trackChanges: false)
                .Include(a => a.Template)
                .Include(a => a.Evaluator)
                .Include(a => a.Evaluatee)
                .FirstOrDefaultAsync();

            return _mapper.Map<AssignmentResponseDto>(createdAssignment);
        }

        public async Task<IEnumerable<AssignmentResponseDto>> GetAllAssignmentsAsync()
        {
            var assignments = await _unitOfWork.EvaluationAssignments
                 .GetAll(trackChanges: false)
                 .Include(a => a.Template)
                 .Include(a => a.Evaluator)
                 .Include(a => a.Evaluatee)
                 .ToListAsync();

            return _mapper.Map<IEnumerable<AssignmentResponseDto>>(assignments);
        }

        public async Task<AssignmentResponseDto> GetAssignmentByIdAsync(int id)
        {
            var assignment = await _unitOfWork.EvaluationAssignments
                 .FindByCondition(a => a.Id == id, trackChanges: false)
                 .Include(a => a.Template)
                 .Include(a => a.Evaluator)
                 .Include(a => a.Evaluatee)
                 .FirstOrDefaultAsync();

            if (assignment == null)
            {
                throw new Exception("Assignment not found");
            }

            return _mapper.Map<AssignmentResponseDto>(assignment);
        }

        public async Task<IEnumerable<AssignmentResponseDto>> GetMyPendingEvaluationsAsync(int evaluatorId)
        {
            var pendingAssignments = await _unitOfWork.EvaluationAssignments
                 .FindByCondition(a => a.EvaluatorId == evaluatorId && a.Status == EvaluationStatus.Pending, trackChanges: false)
                 .Include(a => a.Template)
                 .Include(a => a.Evaluatee)
                 .Include(a => a.Evaluator)
                 .ToListAsync();

            return _mapper.Map<IEnumerable<AssignmentResponseDto>>(pendingAssignments);
        }

        public async Task<AssignmentResponseDto> UpdateAssignmentAsync(int id, CreateAssignmentDto dto)
        {
            var assignment = await _unitOfWork.EvaluationAssignments.GetByIdAsync(id);
            if (assignment == null)
            {
                throw new Exception("Assignment not found");
            }

            _mapper.Map(dto, assignment);

            _unitOfWork.EvaluationAssignments.Update(assignment);
            await _unitOfWork.SaveChangesAsync();

            var updatedAssignment = await _unitOfWork.EvaluationAssignments
                 .FindByCondition(a => a.Id == id, trackChanges: false)
                 .Include(a => a.Template)
                 .Include(a => a.Evaluator)
                 .Include(a => a.Evaluatee)
                 .FirstOrDefaultAsync();

            return _mapper.Map<AssignmentResponseDto>(updatedAssignment);
        }
    }
}