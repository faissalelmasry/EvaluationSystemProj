using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EvaluationSystem.Application.DTOs.EvaluationCriteria;
using EvaluationSystem.Application.DTOs.EvaluationSection;
using EvaluationSystem.Application.interfaces;
using EvaluationSystem.Application.Services.ServiceInterfaces;
using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.Services.CriteriaService
{
    public class CriteriaService : IEvaluationCriteriaService
    {
        readonly IGenericRepo<EvaluationCriteria> CriteriaRepo;
        readonly IGenericRepo<EvaluationSection> SectionRepo;
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public CriteriaService
            (IGenericRepo<EvaluationCriteria> _CriteriaRepo
            , IGenericRepo<EvaluationSection> _SectionRepo
            , IMapper _mapper
            , IUnitOfWork _unitOfWork) 
        {
            CriteriaRepo = _CriteriaRepo;
            SectionRepo = _SectionRepo;
            mapper = _mapper;
            unitOfWork = _unitOfWork;
        }
        public async Task<bool> AddCriteriaAsync(int sectionid, AddEvaluationCriteriaDto dto)
        {
            var section =await SectionRepo.GetByIdAsync(sectionid);
            if (section == null)
                return false;
            var criteria = mapper.Map<EvaluationCriteria>(dto);
            criteria.SectionId = sectionid;
            await CriteriaRepo.AddAsync(criteria);
            var res = await unitOfWork.SaveChangesAsync();
            return res > 0;
        }

        public async Task<bool> DeleteCriteriaAsync(int id)
        {
            var criteria = await CriteriaRepo.GetByIdAsync(id);
            if (criteria == null) 
                return false;
            criteria.IsDeleted = true;
            var res = await unitOfWork.SaveChangesAsync();
            return res > 0;
        }

        public async Task<bool> UpdateCriteriaAsync(int id, AddEvaluationCriteriaDto dto)
        {
            var criteria = await CriteriaRepo.GetByIdAsync(id);

            if (criteria == null)
                return false;

            mapper.Map(dto, criteria);
            var res = await unitOfWork.SaveChangesAsync();
            return res > 0;

        }
    }
}
