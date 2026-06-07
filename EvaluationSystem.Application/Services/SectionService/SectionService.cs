using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EvaluationSystem.Application.DTOs.EvaluationSection;
using EvaluationSystem.Application.DTOs.EvaluationTemplate;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using EvaluationSystem.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace EvaluationSystem.Application.Services.SectionService
{
    public class SectionService : IEvaluationSectionService
    {
        private readonly IGenericRepo<EvaluationSection> SectionRepo;
        private readonly IGenericRepo<EvaluationTemplate> TemplateRepo;

        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        public SectionService
            (IGenericRepo<EvaluationSection> _SectionRepo, IMapper _mapper, IUnitOfWork _unitOfWork, IGenericRepo<EvaluationTemplate> _TemplateRepo)
        {
            SectionRepo = _SectionRepo;
            TemplateRepo = _TemplateRepo;
            mapper = _mapper;
            unitOfWork = _unitOfWork;
        }
        public async Task<bool> AddSectionAsync(int templateId, AddEvaluationSectionDto dto)
        {
            var template = await TemplateRepo.GetByIdAsync(templateId);
            if (template == null)
                return false;
            var section = mapper.Map<EvaluationSection>(dto);

            section.TemplateId = templateId;

            await SectionRepo.AddAsync(section);
            var res = await unitOfWork.SaveChangesAsync();
            return res>0;
        }
        public async Task<bool> UpdateSectionAsync(int id,AddEvaluationSectionDto dto)
        {
            var section= await SectionRepo.GetByIdAsync(id);

            if(section == null) 
                return false;

            mapper.Map(dto, section);
            var res = await unitOfWork.SaveChangesAsync();
            return res>0;

        }
        public async Task<bool> DeleteSectionAsync(int id)
        {
            var section = await SectionRepo.GetByIdAsync(id);

            if (section == null)
                return false;
            section.IsDeleted= true;
            var res = await unitOfWork.SaveChangesAsync();
            return res > 0;

        }

    }
}
