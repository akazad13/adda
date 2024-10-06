using Microsoft.AspNetCore.Http;

namespace Adda.API.Dtos;

public class CreatePhotoRequest
{
    public IFormFile File { get; set; }
}
