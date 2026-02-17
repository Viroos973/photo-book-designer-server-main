using photo_book_designer_server_main.DTO;
using photo_book_designer_server_main.Services.Interfaces;
using System.Text.Json;
using System.Net.Http.Headers;

namespace photo_book_designer_server_main.Services;

public class PhotoStorageClient : IPhotoStorageClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public PhotoStorageClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["PhotoStorage:BaseUrl"] ?? "http://localhost:5001";
        _httpClient.BaseAddress = new Uri(_baseUrl);
    }

    public async Task<PhotoStorageDTO> UploadPhotoAsync(IFormFile file)
    {
        using var content = new MultipartFormDataContent();

        await using var fileStream = file.OpenReadStream();
        var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

        content.Add(streamContent, "file");

        var response = await _httpClient.PostAsync("/api/photos/upload", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new BadHttpRequestException($"Error when uploading a photo: {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<PhotoStorageDTO>(jsonResponse, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result ?? throw new BadHttpRequestException("Failed to parse the response from the photo server");
    }

    public async Task DeletePhotoAsync(string imageId)
    {
        var response = await _httpClient.DeleteAsync($"/api/photos/{imageId}");

        if (!response.IsSuccessStatusCode)
        {
            throw new BadHttpRequestException($"Error when delete a photo: {response.StatusCode}");
        }
    }
}
