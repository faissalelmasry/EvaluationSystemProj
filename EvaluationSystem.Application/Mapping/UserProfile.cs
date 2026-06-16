using AutoMapper;
using EvaluationSystem.Application.DTOs.User;
using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, User>()
    .ForMember(
        dest => dest.UserName,
        opt => opt.MapFrom(src => src.UserName)
    );

            CreateMap<User, UserDto>()
                .ForMember(
                    dest => dest.Department,
                    opt => opt.MapFrom(src => src.Department.Name));
        }
    }
}