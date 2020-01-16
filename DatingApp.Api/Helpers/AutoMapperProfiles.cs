using System.Linq;
using AutoMapper;
using DatingApp.Api.DTOs;
using DatingApp.Api.Models;

namespace DatingApp.Api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserListDto>()
            .ForMember(dest => dest.PhotoURL, 
            src => src.MapFrom(opt => opt.Photos.FirstOrDefault(p => p.IsActive).Url))
            .ForMember(dest => dest.Age, 
            src => src.MapFrom(opt => opt.DateOfBirth.CalculateAge()));
            CreateMap<User, UserDetailsDto>()
             .ForMember(dest => dest.PhotoURL, 
            src => src.MapFrom(opt => opt.Photos.FirstOrDefault(p => p.IsActive).Url))
            .ForMember(dest => dest.Age, 
            src => src.MapFrom(opt => opt.DateOfBirth.CalculateAge()));;
            CreateMap<Photo, PhotosDto>();
            CreateMap<UserUpdateDto,User>();
            CreateMap<PhotoCreateDto,Photo>();
            CreateMap<Photo,PhotoForReturnDto>();
            CreateMap<UserRegistrationDto,User>();
        }
    }
}