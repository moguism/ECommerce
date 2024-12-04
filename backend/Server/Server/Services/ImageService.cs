using Server.DTOs;
using Server.Helpers;

namespace Server.Services;

public class ImageService
{
    private const string IMAGES_FOLDER = "/images/";

    public async Task<string> InsertAsync(IFormFile file)
    {
        string relativePath = $"/{IMAGES_FOLDER}{Guid.NewGuid()}_{file.FileName}";

        await StoreImageAsync(relativePath, file);

        return relativePath;
    }

    private async Task StoreImageAsync(string relativePath, IFormFile file)
    {
        using Stream stream = file.OpenReadStream();

        await FileHelper.SaveAsync(stream, relativePath);
    }
}
