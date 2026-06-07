using AutoMapper;
using EvaluationSystem.Application.DTOs.User;
using EvaluationSystem.Domain.Models;

namespace EvaluationSystem.Application.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, User>();

            CreateMap<User, UserDto>()
                .ForMember(
                    dest => dest.Department,
                    opt => opt.MapFrom(src => src.Department.Name));
        }
    }
}