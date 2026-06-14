using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EvaluationSystem.Application.DTOs.EvaluationCriteria;
using EvaluationSystem.Application.DTOs.EvaluationSection;
using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.Mapping
{
    public class EvaluationCriteriaProfile : Profile
    {
        public EvaluationCriteriaProfile()
        {
            CreateMap<EvaluationCriteria, EvaluationCriteriaDto>().ReverseMap();
            CreateMap<EvaluationCriteria, AddEvaluationCriteriaDto>().ReverseMap();
            CreateMap<EvaluationCriteria, AddTemplateCriteriaDto>().ReverseMap();
        }
    }
}
