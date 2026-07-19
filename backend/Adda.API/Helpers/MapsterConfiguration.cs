using Adda.API.Dtos;
using Adda.API.Models;
using Mapster;

namespace Adda.API.Helpers;

public static class MapsterConfiguration
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<User, UserListDetails>
            .NewConfig()
            .Map(dest => dest.PhotoUrl, src => src.Photos.FirstOrDefault(p => p.IsMain)!.Url)
            .Map(dest => dest.Age, src => src.DateOfBirth.CalculateAge());

        TypeAdapterConfig<User, UserDetails>
            .NewConfig()
            .Map(dest => dest.Photos, src => src.Photos)
            .Map(dest => dest.PhotoUrl, src => src.Photos.FirstOrDefault(p => p.IsMain)!.Url)
            .Map(dest => dest.Age, src => src.DateOfBirth.CalculateAge());

        TypeAdapterConfig<Photo, PhotosDetails>.NewConfig();
        TypeAdapterConfig<UserUpdateRequest, User>.NewConfig();
        TypeAdapterConfig<Photo, PhotoResponse>.NewConfig();
        TypeAdapterConfig<RegistrationRequest, User>.NewConfig();
        TypeAdapterConfig<CreateMessageRequest, Message>.NewConfig();

        TypeAdapterConfig<Message, MessageResponse>
            .NewConfig()
            .Map(dest => dest.SenderPhotoUrl, src => src.Sender.Photos.FirstOrDefault(p => p.IsMain)!.Url)
            .Map(dest => dest.RecipientPhotoUrl, src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain)!.Url);
    }
}
