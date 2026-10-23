using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Interfaces.Repository
{
    public interface IFileRepository
    {
        Task<int> GetLastFileId();
        Task AddFileAsync(MediaFile file);
        Task<MediaFile> GetFileByIdAsync(int id);
        Task<MediaFile?> FindFileByIdAsync(int id);
        Task<List<MediaFile>> GetFilesByArticleIdAsync(int articleId);
        Task AttachFileToArticleAsync(int fileId, int articleId);
        Task RemoveFileAsync(int id);
    }
}
