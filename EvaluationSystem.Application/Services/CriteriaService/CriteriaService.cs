using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EvaluationSystem.Application.DTOs.EvaluationCriteria;
using EvaluationSystem.Application.DTOs.EvaluationSection;
using EvaluationSystem.Application.Exceptions;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using EvaluationSystem.Domain.Exceptions;
using EvaluationSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

namespace EvaluationSystem.Application.Services.CriteriaService
{
    public class CriteriaService : IEvaluationCriteriaService
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public CriteriaService
            (IMapper _mapper,IUnitOfWork _unitOfWork) 
        {
            mapper = _mapper;
            unitOfWork = _unitOfWork;
        }
        public async Task AddCriteriaAsync(int sectionid, AddEvaluationCriteriaDto dto)
        {
            var section =await unitOfWork.EvaluationSections.GetByIdAsync(sectionid);
            if (section == null)
                throw new NotFoundException("Section isn't found ");
            var conflict = await unitOfWork.EvaluationCriterias
                .FindByCondition(c =>
                    c.SectionId == sectionid &&
                    (c.Title == dto.Title || c.OrderNo == dto.OrderNo))
                .Select(c => new { c.Title, c.OrderNo })
                .FirstOrDefaultAsync();

            if (conflict != null)
            {
                if (conflict.Title == dto.Title)
                    throw new BadRequestException("Criteria already exists in this section");
                if (conflict.OrderNo == dto.OrderNo)
                    throw new BadRequestException("Order already exists in this section");
            }
            var criteria = mapper.Map<EvaluationCriteria>(dto);
            criteria.SectionId = sectionid;
            await unitOfWork.EvaluationCriterias.AddAsync(criteria);
            var AffectedRows = await unitOfWork.SaveChangesAsync();
            if (AffectedRows == 0)
                throw new BadRequestException("criteria isn't added");
        }

        public async Task DeleteCriteriaAsync(int id)
        {
            var criteria = await unitOfWork.EvaluationCriterias.GetByIdAsync(id);
            if (criteria == null) 
                throw new NotFoundException("Criteria is not found");
            var HasResponses= await unitOfWork.EvaluationResponses.GetAll().AnyAsync(r => r.CriterionId == id);
            if (HasResponses)
                throw new BadRequestException("can't delete this criteria because it has responses");
            criteria.IsDeleted = true;
            criteria.DeletedAt = DateTime.UtcNow;
            var AffectedRows = await unitOfWork.SaveChangesAsync();
            if(AffectedRows==0)
                throw new BadRequestException("can't delete this criteria");
        }

        public async Task UpdateCriteriaAsync(int id, AddEvaluationCriteriaDto dto)
        {
            var criteria = await unitOfWork.EvaluationCriterias.GetByIdAsync(id);

            if (criteria == null)
                throw new NotFoundException("Criteria is not found");
            var conflict = await unitOfWork.EvaluationCriterias
                .FindByCondition(c =>
                    c.SectionId == criteria.SectionId && c.Id != id &&
                    (c.Title == dto.Title || c.OrderNo == dto.OrderNo))
                .Select(c => new { c.Title, c.OrderNo })
                .FirstOrDefaultAsync();
            if (conflict != null)
            {
                if (conflict.Title == dto.Title)
                    throw new BadRequestException("Criteria already exists in this section");
                if (conflict.OrderNo == dto.OrderNo)
                    throw new BadRequestException("Order already exists in this section");
            }
            mapper.Map(dto, criteria);
            var AffectedRows = await unitOfWork.SaveChangesAsync();
            if(AffectedRows == 0)
                throw new BadRequestException("no updates happened in criteria");

        }
    }
}
