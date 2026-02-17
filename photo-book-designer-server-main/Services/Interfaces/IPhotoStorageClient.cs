using photo_book_designer_server_main.DTO;

namespace photo_book_designer_server_main.Services.Interfaces;

public interface IPhotoStorageClient
{
    Task<PhotoStorageDTO> UploadPhotoAsync(IFormFile file);
    Task DeletePhotoAsync(string imageId);
}
