using Adda.API.Dtos;
using Adda.API.Models;
using AutoMapper;

namespace Adda.API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<User, UserListDetails>()
            .ForMember(
                dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url)
            )
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<User, UserDetails>()
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photos))
            .ForMember(
                dest => dest.PhotoUrl,
                opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url)
            )
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotosDetails>();
        CreateMap<UserUpdateRequest, User>();
        CreateMap<Photo, PhotoResponse>();

        CreateMap<RegistrationRequest, User>();
        CreateMap<CreateMessageRequest, Message>();
        CreateMap<Message, MessageResponse>()
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
