using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class FileService : IFileService
    {
        private const long MaxFileSize = 5 * 1024 * 1024;

        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        private readonly IFileRepository _fileRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;
        private readonly string _uploadPath;

        public FileService(
            IFileRepository fileRepository,
            IWebHostEnvironment environment,
            IPermissionService permissionService,
            INotificationService notificationService)
        {
            _fileRepository = fileRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
            _uploadPath = Path.Combine(environment.ContentRootPath, "Images");
        }

        public async Task<string> GetPreviewAsync(IBrowserFile file)
        {
            using var stream = file.OpenReadStream(MaxFileSize);
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            byte[] bytes = memoryStream.ToArray();
            return $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
        }

        public async Task<int> UploadImage(IBrowserFile file, int? ArticleId = null)
        {
            await EnsureAddPermissionAsync();

            string? error = ValidateImage(file);
            if (error != null)
            {
                _notificationService.Show($"{file.Name}: {error}", EnumNotificationType.Failure);
                return 0;
            }

            return await SaveImageAsync(file, ArticleId);
        }

        public async Task<List<int>> UploadImagesAsync(IEnumerable<IBrowserFile> files, int? articleId = null)
        {
            await EnsureAddPermissionAsync();

            var ids = new List<int>();
            foreach (var file in files)
            {
                string? error = ValidateImage(file);
                if (error != null)
                {
                    _notificationService.Show($"{file.Name}: {error}", EnumNotificationType.Failure);
                    continue;
                }

                ids.Add(await SaveImageAsync(file, articleId));
            }

            return ids;
        }

        public async Task AttachImagesToArticleAsync(IEnumerable<int> fileIds, int articleId)
        {
            foreach (int fileId in fileIds)
            {
                await _fileRepository.AttachFileToArticleAsync(fileId, articleId);
            }
        }

        public async Task<bool> DeleteImageAsync(int fileId)
        {
            bool canAdd = await _permissionService.HasPermissionAsync(EnumPermissions.AddArticle);
            bool canEdit = await _permissionService.HasPermissionAsync(EnumPermissions.EditArticle);
            if (!canAdd && !canEdit)
            {
                _notificationService.Show("Brak uprawnien!", EnumNotificationType.Failure);
                return false;
            }

            var file = await _fileRepository.FindFileByIdAsync(fileId);
            if (file == null)
            {
                _notificationService.Show("Nie znaleziono zdjęcia!", EnumNotificationType.Error);
                return false;
            }

            string fullPath = Path.Combine(_uploadPath, file.FileName);
            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
            catch (IOException)
            {
                _notificationService.Show("Nie udało się usunąć pliku z dysku!", EnumNotificationType.Error);
                return false;
            }

            await _fileRepository.RemoveFileAsync(fileId);
            return true;
        }

        public async Task<string> GetFileNameByIdAsync(int id)
        {
            var file = await _fileRepository.GetFileByIdAsync(id);
            return file.FileName;
        }

        public async Task<List<string>> GetImagesByArticleIdAsync(int articleId)
        {
            var files = await _fileRepository.GetFilesByArticleIdAsync(articleId);

            var images = new List<string>();
            foreach (var file in files)
            {
                images.Add($"/Images/{file.FileName}");
            }

            return images;
        }

        public Task<List<MediaFile>> GetFilesByArticleIdAsync(int articleId)
        {
            return _fileRepository.GetFilesByArticleIdAsync(articleId);
        }

        private async Task EnsureAddPermissionAsync()
        {
            bool hasPermission = await _permissionService.HasPermissionAsync(EnumPermissions.AddArticle);
            if (!hasPermission)
            {
                _notificationService.Show("Brak uprawnien!", EnumNotificationType.Failure);
                throw new Exception("Brak uprawnien!");
            }
        }

        private static string? ValidateImage(IBrowserFile file)
        {
            string extension = Path.GetExtension(file.Name);
            if (!AllowedExtensions.Contains(extension))
            {
                return "niedozwolony format pliku";
            }

            if (file.Size > MaxFileSize)
            {
                return "plik jest większy niż 5 MB";
            }

            return null;
        }

        private async Task<int> SaveImageAsync(IBrowserFile file, int? articleId)
        {
            Directory.CreateDirectory(_uploadPath);

            string extension = Path.GetExtension(file.Name);
            string fileName = $"{Guid.NewGuid()}{extension}";
            string fullPath = Path.Combine(_uploadPath, fileName);

            using (var stream = file.OpenReadStream(MaxFileSize))
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            int id = await _fileRepository.GetLastFileId();
            var mediaFile = new MediaFile
            {
                Id = id,
                FileName = fileName,
                FileType = extension,
                ArticleId = articleId,
                IsMainImage = articleId == null
            };

            try
            {
                await _fileRepository.AddFileAsync(mediaFile);
            }
            catch
            {
                // Nie zostawiamy osieroconego pliku na dysku, jeśli zapis do bazy się nie udał
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                throw;
            }

            return id;
        }
    }
}
