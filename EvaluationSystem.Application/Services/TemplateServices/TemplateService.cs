using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EvaluationSystem.Application.DTOs.EvaluationSection;
using EvaluationSystem.Application.DTOs.EvaluationTemplate;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Application.Services.SectionService;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using EvaluationSystem.Domain.Models;
using EvaluationSystem.Domain.Exceptions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using EvaluationSystem.Application.Exceptions;
using static System.Collections.Specialized.BitVector32;
using EvaluationSystem.Application.Helpers;
using EvaluationSystem.Application.DTOs.EvaluationCriteria;
using EvaluationSystem.Domain.Exceptions;

namespace EvaluationSystem.Application.Services.TemplateServices
{
    public class TemplateService : IEvaluationTemplateService
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        public TemplateService(IMapper _mapper,IUnitOfWork _unitOfWork) 
        { 
            mapper = _mapper;
            unitOfWork = _unitOfWork;
        }
        public async Task<List<EvaluationTemplateListDto>> GetTemplatesAsync(int PageNumber = 1, int PageSize = 10, string? Search = "")
        {
            IQueryable<EvaluationTemplate> Templates = unitOfWork.EvaluationTemplates.GetAll().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                Templates = Templates.Where(t => t.Title.Contains(Search) || t.Description.Contains(Search));
            }
            var TempLatesList = await Templates.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToListAsync();
            if(TempLatesList.Count == 0)
                throw new NotFoundException("No templates found");
            var temps= mapper.Map<List<EvaluationTemplateListDto>>(TempLatesList);

            return temps;
        }
        public async Task<GetEvaluationTemplateDto> GetTemplateAsync(int id)
        {
            var temp = await unitOfWork.EvaluationTemplates.GetByIdAsync(id,
                q => q.Include(t => t.EvaluationSections)
                       .ThenInclude(s => s.Criterias)
                );
            if(temp==null)
                throw new NotFoundException("Template isn't found");

            return mapper.Map<GetEvaluationTemplateDto>(temp);
        }
        public async Task AddTemplateAsync(EvaluationTemplateDto dto)
        {
            var conflict = await unitOfWork.EvaluationTemplates
                .FindByCondition(c =>
                    (c.Title == dto.Title))
                .FirstOrDefaultAsync();
            if (conflict != null)
                throw new BadRequestException("Template already exists");
            await unitOfWork.EvaluationTemplates.AddAsync(mapper.Map<EvaluationTemplate>(dto));
            var AffectedRows=await unitOfWork.SaveChangesAsync();
            if(AffectedRows==0)
                throw new BadRequestException("can't add template");
        }
        public async Task AddFullTemplateAsync(AddFullTemplateDto dto)
        {
            var conflict = await unitOfWork.EvaluationTemplates
                .FindByCondition(c =>
                    (c.Title == dto.Title))
                .FirstOrDefaultAsync();
            if (conflict != null)
                throw new BadRequestException("Template already exists");
            TemplateValidationHelper.ValidateNewSections(dto.Sections);
            TemplateValidationHelper.ValidateNewCriteria(dto.Sections);
            await unitOfWork.EvaluationTemplates.AddAsync(mapper.Map<EvaluationTemplate>(dto));
            var AffectedRows = await unitOfWork.SaveChangesAsync();
            if (AffectedRows == 0)
                throw new BadRequestException("can't add template");
        }

        public async Task UpdateTemplateAsync(int id,EvaluationTemplateDto dto)
        {
            var conflict = await unitOfWork.EvaluationTemplates
                .FindByCondition(c =>
                    (c.Title == dto.Title))
                .FirstOrDefaultAsync();
            if (conflict != null)
                throw new BadRequestException("Template already exists");
            TemplateValidationHelper.ValidateNewSections(dto.Sections);
            TemplateValidationHelper.ValidateNewCriteria(dto.Sections);
            await unitOfWork.EvaluationTemplates.AddAsync(mapper.Map<EvaluationTemplate>(dto));
            var AffectedRows = await unitOfWork.SaveChangesAsync();
            if (AffectedRows == 0)
                throw new BadRequestException("can't add template");
        }
        public async Task UpdateTemplateAsync(int templateId, UpdateEvaluationTemplateDto dto)
        {
            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // ── 1. Load the full template from DB ──
                var template = await unitOfWork.EvaluationTemplates
                    .FindByCondition(t => t.Id == templateId)
                    .Include(t => t.EvaluationSections)
                        .ThenInclude(s => s.Criterias)
                    .FirstOrDefaultAsync();

                if (template == null)
                    throw new NotFoundException("Template isn't found");

                // ── 2. Check title uniqueness (excluding itself) ──
                var titleConflict = await unitOfWork.EvaluationTemplates
                    .FindByCondition(t => t.Title == dto.Title && t.Id != templateId)
                    .AnyAsync();

                if (titleConflict)
                    throw new BadRequestException("Template title already exists");

                // ── 3. Batch load existing sections & criteria (2 queries only) ──
                var existingSections = await unitOfWork.EvaluationSections
                    .FindByCondition(s => s.TemplateId == templateId)
                    .Select(s => new SectionLookupDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        OrderNo = s.OrderNo
                    })
                    .ToListAsync();

                var sectionIds = template.EvaluationSections.Select(s => s.Id).ToList();

                var existingCriteria = await unitOfWork.EvaluationCriterias
                    .FindByCondition(c => sectionIds.Contains(c.SectionId))
                    .Select(c => new CriterionLookupDto
                    {
                        Id = c.Id,
                        SectionId = c.SectionId,
                        Title = c.Title,
                        OrderNo = c.OrderNo
                    })
                    .ToListAsync();

                // ── 4. Validate duplicates in-memory ──
                TemplateValidationHelper.ValidateSections(dto.Sections, existingSections);
                TemplateValidationHelper.ValidateCriteria(dto.Sections, existingCriteria);

                // ── 5. Update template fields ──
                template.Title = dto.Title;
                template.Description = dto.Description;

                // ── 6. Sync sections (Add / Update / Delete) ──
                SyncSections(template, dto.Sections);

                // ── 7. Save everything in one shot ──
                var affectedRows = await unitOfWork.SaveChangesAsync();
                if (affectedRows == 0)
                    throw new BadRequestException("Can't update template");
            });
        }


        public async Task UpdateTemplateAsync(int id,EvaluationTemplateDto dto)
        {
            var temp = await unitOfWork.EvaluationTemplates.GetByIdAsync(id);
            if (temp == null)
                throw new NotFoundException("Template isn't found");
            var exists = await unitOfWork.EvaluationTemplates
                .FindByCondition(t => t.Title == dto.Title && t.Id != id)
                .AnyAsync();
            if(exists)
                throw new BadRequestException("Template already exists");

            mapper.Map(dto, temp);

            var AffectedRows = await unitOfWork.SaveChangesAsync();
            if (AffectedRows == 0)
                throw new BadRequestException("can't update template");
        }
        public async Task DeleteTemplateAsync(int id)
        {
            
            var temp = await unitOfWork.EvaluationTemplates.GetByIdAsync(id);
            if (temp == null)
                throw new NotFoundException("Template isn't found");
            var HasAssignments= await unitOfWork.EvaluationAssignments.FindByCondition(a => a.TemplateId == id).AnyAsync();
            if(HasAssignments)
                throw new BadRequestException("can't delete template because it has assignments");
            var HasSections = await unitOfWork.EvaluationSections.FindByCondition(a => a.TemplateId == id).AnyAsync();
            if (HasSections)
                throw new BadRequestException("can't delete template because it has sections");

            temp.IsDeleted=true;
            temp.DeletedAt = DateTime.UtcNow;
            var AffectedRows = await unitOfWork.SaveChangesAsync();
            if(AffectedRows==0)
                throw new BadRequestException("can't delete template");
        }
        private void SyncSections(EvaluationTemplate template, List<UpdateSectionDto> sectionDtos)
        {
            // IDs اللي جايين من الـ frontend (اللي ليهم id بس)
            var incomingIds = sectionDtos
                .Where(s => s.Id.HasValue)
                .Select(s => s.Id!.Value)
                .ToList();

            // ── DELETE: أي section موجودة في DB بس مش في الـ payload ──
            var sectionsToDelete = template.EvaluationSections
                .Where(s => !incomingIds.Contains(s.Id))
                .ToList();

            foreach (var section in sectionsToDelete)
                unitOfWork.EvaluationSections.Delete(section);
            // Cascade delete هيمسح الـ criteria بتاعتها تلقائياً

            // ── UPDATE & INSERT ──
            foreach (var sectionDto in sectionDtos)
            {
                if (sectionDto.Id.HasValue)
                {
                    // ── UPDATE: section موجودة ──
                    var section = template.EvaluationSections
                        .FirstOrDefault(s => s.Id == sectionDto.Id.Value);

                    if (section == null)
                        throw new NotFoundException($"Section {sectionDto.Id} not found");

                    section.Title = sectionDto.Title;
                    section.Description = sectionDto.Description;
                    section.OrderNo = sectionDto.OrderNo;

                    // Sync الـ criteria بتاعتها
                    SyncCriteria(section, sectionDto.Criteria);
                }
                else
                {
                    // ── INSERT: section جديدة (id = null) ──
                    var newSection = mapper.Map<EvaluationSection>(sectionDto);
                    newSection.TemplateId = template.Id;
                    newSection.Criterias = sectionDto.Criteria
                        .Select(c => mapper.Map<EvaluationCriteria>(c))
                        .ToList();

                    template.EvaluationSections.Add(newSection);
                }
            }
        }
        private void SyncCriteria(EvaluationSection section, List<UpdateCriterionDto> criteriaDtos)
        {
            var incomingIds = criteriaDtos
                .Where(c => c.Id.HasValue)
                .Select(c => c.Id!.Value)
                .ToList();

            // ── DELETE: criteria موجودة في DB بس مش في الـ payload ──
            var criteriaToDelete = section.Criterias
                .Where(c => !incomingIds.Contains(c.Id))
                .ToList();

            foreach (var criterion in criteriaToDelete)
                unitOfWork.EvaluationCriterias.Delete(criterion);

            // ── UPDATE & INSERT ──
            foreach (var criterionDto in criteriaDtos)
            {
                if (criterionDto.Id.HasValue)
                {
                    // ── UPDATE ──
                    var criterion = section.Criterias
                        .FirstOrDefault(c => c.Id == criterionDto.Id.Value);

                    if (criterion == null)
                        throw new NotFoundException($"Criterion {criterionDto.Id} not found");

                    criterion.Title = criterionDto.Title;
                    criterion.OrderNo = criterionDto.OrderNo;
                    criterion.QuestionType = criterionDto.QuestionType;
                    criterion.MaxScore = criterionDto.MaxScore;
                    criterion.Weight = criterionDto.Weight;
                    criterion.IsRequired = criterionDto.IsRequired;
                }
                else
                {
                    // ── INSERT ──
                    var newCriterion = mapper.Map<EvaluationCriteria>(criterionDto);
                    newCriterion.SectionId = section.Id;
                    section.Criterias.Add(newCriterion);
                }
            }
        }

    }
}
