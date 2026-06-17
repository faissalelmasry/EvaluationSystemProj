using AutoMapper;
using EvaluationSystem.Application.DTOs.Assignments;
using EvaluationSystem.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluationSystem.Application.Mapping
{
   public class EvaluationAssignmentProfile:Profile
    {
        public EvaluationAssignmentProfile()
        {

            CreateMap<EvaluationAssignment, AssignmentResponseDto>()


                .ForMember(dest => dest.TemplateTitle,
                           opt => opt.MapFrom(src => src.Template != null ? src.Template.Title : string.Empty))


                .ForMember(dest => dest.EvaluatorName,
                           opt => opt.MapFrom(src => src.Evaluator != null ? src.Evaluator.FullName : string.Empty))


                .ForMember(dest => dest.EvaluateeName,
                           opt => opt.MapFrom(src => src.Evaluatee != null ? src.Evaluatee.FullName : string.Empty))
                .ForMember(dest=> dest.EvaluateeId,opt => opt.MapFrom(src => src.EvaluateeId))
                .ForMember(dest => dest.EvaluatorId, opt => opt.MapFrom(src => src.EvaluatorId));
            CreateMap<CreateAssignmentDto, EvaluationAssignment>();
        }

    }
}
