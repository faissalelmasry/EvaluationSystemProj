using AutoMapper;
using EvaluationSystem.Application.DTOs.Auth;
using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.Mapping
{
    public class AuthProfile:Profile
    {
        public AuthProfile() {
            CreateMap<RegisterDTO, User>().ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                ;
        }
    }
}
