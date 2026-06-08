using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EvaluationSystem.Application.DTOs.EvaluationSection;
using EvaluationSystem.Application.DTOs.EvaluationTemplate;
using EvaluationSystem.Application.Exceptions;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using EvaluationSystem.Domain.Exceptions;
using EvaluationSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvaluationSystem.Application.Services.SectionService
{
    public class SectionService : IEvaluationSectionService
    {

        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        public SectionService
            (IGenericRepo<EvaluationSection> _SectionRepo, IMapper _mapper, IUnitOfWork _unitOfWork, IGenericRepo<EvaluationTemplate> _TemplateRepo)
        {
            mapper = _mapper;
            unitOfWork = _unitOfWork;
        }
        public async Task AddSectionAsync(int templateId, AddEvaluationSectionDto dto)
        {
            var template = await unitOfWork.EvaluationTemplates.GetByIdAsync(templateId);
            if (template == null)
                throw new NotFoundException("Template isn't found");
            var exists = await unitOfWork.EvaluationSections
                .FindByCondition(s => s.TemplateId == templateId && (s.Title == dto.Title || s.OrderNo == dto.OrderNo))
                .Select(s => new { s.Title, s.OrderNo }).FirstOrDefaultAsync();
            if (exists != null)
            {
                if (exists.Title == dto.Title)
                    throw new BadRequestException("Section already exists in this template");
                if (exists.OrderNo == dto.OrderNo)
                    throw new BadRequestException("Order already exists in this template");
            }
            var section = mapper.Map<EvaluationSection>(dto);

            section.TemplateId = templateId;

            await unitOfWork.EvaluationSections.AddAsync(section);
            var AffectedRows = await unitOfWork.SaveChangesAsync();
            if(AffectedRows==0)
                throw new BadRequestException("Section isn't added");
        }
        public async Task UpdateSectionAsync(int id,AddEvaluationSectionDto dto)
        {
            var section= await unitOfWork.EvaluationSections.GetByIdAsync(id);

            if(section == null) 
                throw new NotFoundException("Section isn't found");
            var exists = await unitOfWork.EvaluationSections
                .FindByCondition(s => s.TemplateId == section.TemplateId && s.Id!=id && (s.Title == dto.Title || s.OrderNo == dto.OrderNo))
                .Select(s => new { s.Title, s.OrderNo }).FirstOrDefaultAsync();
            if (exists != null)
            {
                if (exists.Title == dto.Title)
                    throw new BadRequestException("Section already exists in this template");
                if (exists.OrderNo == dto.OrderNo)
                    throw new BadRequestException("Order already exists in this template");
            }

            mapper.Map(dto, section);
            var AffectedRows = await unitOfWork.SaveChangesAsync();
            if(AffectedRows==0)
                throw new BadRequestException("Section isn't updated");

        }
        public async Task DeleteSectionAsync(int id)
        {
            var section = await unitOfWork.EvaluationSections.GetByIdAsync(id);

            if (section == null)
                throw new NotFoundException("Section isn't found");
           var HasCriterias = await unitOfWork.EvaluationCriterias.FindByCondition(c => c.SectionId == id).AnyAsync();
            if (HasCriterias)
                throw new BadRequestException("Section can't be deleted because it has criterias");
            section.IsDeleted= true;
            section.DeletedAt = DateTime.UtcNow;
            var AffectedRows = await unitOfWork.SaveChangesAsync();
            if(AffectedRows==0)
                throw new BadRequestException("Section isn't deleted");

        }

    }
}
