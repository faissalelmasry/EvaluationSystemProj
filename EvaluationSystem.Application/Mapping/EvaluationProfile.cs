using AutoMapper;
using EvaluationSystem.Application.DTOs.Evaluation_Response;
using EvaluationSystem.Application.DTOs.Evaluation_Result;
using EvaluationSystem.Application.DTOs.Evaluation_Reviewer;
using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.Mapping
{
    public class EvaluationProfile : Profile
    {
        public EvaluationProfile()
        {
            CreateMap<QuestionResponseDto, EvaluationResponse>()
                .ForMember(dest => dest.AssignmentId, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Assignment, opt => opt.Ignore())
                .ForMember(dest => dest.Criterion, opt => opt.Ignore());

            CreateMap<EvaluationResult, EvaluationResultDto>();

            CreateMap<EvaluationReview, EvaluationReviewDto>();
        }
    }
}