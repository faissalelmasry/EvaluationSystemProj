using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EvaluationSystem.Application.DTOs.EvaluationTemplate;
using EvaluationSystem.Domain.BaseModels;
using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.Mapping
{
    public class EvaluationTemplateProfile:Profile
    {
        public EvaluationTemplateProfile()
        {
            CreateMap<EvaluationTemplate, EvaluationTemplateListDto>();
            CreateMap<EvaluationTemplate, GetEvaluationTemplateDto>()
                .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.EvaluationSections));
            CreateMap<EvaluationTemplate, EvaluationTemplateDto>().ReverseMap();
        }
    }
}
