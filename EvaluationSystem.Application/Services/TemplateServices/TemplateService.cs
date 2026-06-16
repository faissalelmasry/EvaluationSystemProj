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
using EvaluationSystem.Application.Exceptions;
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
            var temp = await unitOfWork.EvaluationTemplates.GetByIdAsync(id);
            if (temp == null)
                throw new NotFoundException("Template isn't found");
            var exists = await unitOfWork.EvaluationTemplates
                .GetAll().Where(t => t.Title == dto.Title && t.Id != id)
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
     

    }
}
