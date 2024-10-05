using AutoMapper;
using EasyConnect.API.Models;
using EasyConnect.API.Dtos;
using System.Linq;

namespace EasyConnect.API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<User, UserForListDto>()
            .ForMember(
                dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url)
            )
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<User, UserForDetailedDto>()
            .ForMember(
                dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url)
            )
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotosForDetailedDto>();
        CreateMap<UserForUpdateDto, User>();
        CreateMap<PhotoForCreationDto, Photo>();
        CreateMap<Photo, PhotoForReturnDto>(); // from, to

        CreateMap<UserForRegisterDto, User>();
        CreateMap<MessageForCreationDto, Message>();
        CreateMap<Message, MessageToReturnDto>()
            .ForMember(
                m => m.SenderPhotoUrl,
                opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url)
            )
            .ForMember(
                m => m.RecipientPhotoUrl,
                opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url)
            );
    }
}
