using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(
                    destinationMember => destinationMember.PhotoUrl,
                    expression => expression.MapFrom(user => user.Photos.FirstOrDefault(photo => photo.IsMain).Url)
                )
                .ForMember(
                    destinationMember => destinationMember.Age,
                    expression => expression.ResolveUsing(user => user.DateOfBirth.CalculateAge())
                );
            CreateMap<User, UserForDetailedDto>()
                .ForMember(
                    destinationMember => destinationMember.PhotoUrl,
                    expression => expression.MapFrom(user => user.Photos.FirstOrDefault(photo => photo.IsMain).Url)
                )
                .ForMember(
                    destinationMember => destinationMember.Age,
                    expression => expression.ResolveUsing(user => user.DateOfBirth.CalculateAge())
                );
            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserForRegisterDto, User>();
        }
    }
}