using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EvaluationSystem.Application.DTOs.EvaluationSection;
using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.Mapping
{
    public class EvaluationSectionProfile:Profile
    {
        public EvaluationSectionProfile() 
        {
            CreateMap<EvaluationSection,EvaluationSectionDto>().ForMember(dest=>dest.Criteria,opt => opt.MapFrom(src => src.Criterias)).ReverseMap();
            CreateMap<EvaluationSection, AddEvaluationSectionDto>().ReverseMap();
            CreateMap<EvaluationSection,EvaluationTemplateSectionsDto>()
                .AfterMap((src, dest) => { dest.TemplateTitle = src.Template.Title; }).ReverseMap();
            CreateMap<EvaluationSection, AddTemplateSectionDto>().ForMember(dest => dest.Criteria, opt => opt.MapFrom(src => src.Criterias)).ReverseMap();

        }
    }
}
