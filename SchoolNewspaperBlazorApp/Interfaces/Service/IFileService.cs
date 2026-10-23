using Microsoft.AspNetCore.Components.Forms;
using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface IFileService
    {
        Task<string> GetPreviewAsync(IBrowserFile file);
        Task<int> UploadImage(IBrowserFile file, int? Article = null);
        Task<List<int>> UploadImagesAsync(IEnumerable<IBrowserFile> files, int? articleId = null);
        Task AttachImagesToArticleAsync(IEnumerable<int> fileIds, int articleId);
        Task<bool> DeleteImageAsync(int fileId);
        Task<string> GetFileNameByIdAsync(int id);
        Task<List<string>> GetImagesByArticleIdAsync(int articleId);
        Task<List<MediaFile>> GetFilesByArticleIdAsync(int articleId);
    }
}
