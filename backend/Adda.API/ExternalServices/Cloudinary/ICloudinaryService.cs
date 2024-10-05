using System.Threading.Tasks;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace Adda.API.ExternalServices.Cloudinary;

public interface ICloudinaryService
{
    Task<ErrorOr<PhotoUploadResult>> UploadPhotoAsync(IFormFile file);
    Task<ErrorOr<Success>> DeletePhotoAsync(string publicId);
}
