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
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EvaluationSystem.Application.Services.TemplateServices
{
    public class TemplateService : IEvaluationTemplateService
    {
        private readonly IGenericRepo<EvaluationTemplate> EvaluationRepo;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        public TemplateService(IGenericRepo<EvaluationTemplate> _EvaluationRepo,IMapper _mapper,IUnitOfWork _unitOfWork) 
        { 
            EvaluationRepo = _EvaluationRepo;
            mapper = _mapper;
            unitOfWork = _unitOfWork;
        }
        public async Task<List<EvaluationTemplateListDto>> GetTemplatesAsync(int PageNumber = 1, int PageSize = 10, string? Search = "")
        {
            IQueryable<EvaluationTemplate> Templates = EvaluationRepo.GetAll().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                Templates = Templates.Where(t => t.Title.Contains(Search) || t.Description.Contains(Search));
            }
            var TempLatesList = await Templates.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToListAsync();
            var temps= mapper.Map<List<EvaluationTemplateListDto>>(TempLatesList);

            return temps;
        }
        public async Task<GetEvaluationTemplateDto> GetTemplateAsync(int id)
        {
            var temp = await EvaluationRepo.GetByIdAsync(id,
                q => q.Include(t => t.EvaluationSections)
                       .ThenInclude(s => s.Criterias)
                );
            return mapper.Map<GetEvaluationTemplateDto>(temp);
        }
        public async Task<bool> AddTemplateAsync(EvaluationTemplateDto dto)
        {
            await EvaluationRepo.AddAsync(mapper.Map<EvaluationTemplate>(dto));
            var res=await unitOfWork.SaveChangesAsync();
            return res > 0;

        }
        public async Task<bool> UpdateTemplateAsync(int id,EvaluationTemplateDto dto)
        {
            var temp = await EvaluationRepo.GetByIdAsync(id);
            if (temp == null)
                return false;

            mapper.Map(dto, temp);

            var res = await unitOfWork.SaveChangesAsync();
            return res > 0;
        }
        public async Task<bool> DeleteTemplateAsync(int id)
        {
            
            var temp = await EvaluationRepo.GetByIdAsync(id);
            if (temp == null)
                return false;
            temp.IsDeleted=true;
            var res = await unitOfWork.SaveChangesAsync();
            return res>0;
        }

    }
}
