using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ErrorOr;

namespace Adda.API.ExternalServices.Cloudinary;

public class CloudinaryService(
    ICloudinary cloudinary
    ) : ICloudinaryService
{
    private readonly ICloudinary _cloudinary = cloudinary;

    public async Task<ErrorOr<PhotoUploadResult>> UploadPhotoAsync(IFormFile file)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(file);

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face"),
                    Folder = "adda"
                };

                try
                {
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult.Error == null && !string.IsNullOrEmpty(uploadResult.Url?.ToString()))
                    {
                        return new PhotoUploadResult(uploadResult.Url.ToString(), uploadResult.PublicId);
                    }
                }
                catch
                {
                    // Fall back to local file storage if Cloudinary throws an exception
                }

                // Local storage fallback
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                string localUrl = $"/uploads/{uniqueFileName}";
                return new PhotoUploadResult(localUrl, uniqueFileName);
            }
            return ErrorOr.Error.Failure(description: "File is empty!");
        }
        catch (Exception ex)
        {
            return ErrorOr.Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<Success>> DeletePhotoAsync(string publicId)
    {
        try
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result.Result switch
            {
                "ok" => Result.Success,
                _ => ErrorOr.Error.Failure(description: result.Error.Message),
            };
        }
        catch (Exception ex)
        {
            return ErrorOr.Error.Failure(description: ex.Message);
        }
    }
}

public record PhotoUploadResult(string Url, string PublicId);
