using Microsoft.AspNetCore.Components.Forms;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class FileService : IFileService
    {
        private readonly IFileRepository fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            this.fileRepository = fileRepository;
        }

        private readonly string uploadPath =
            @"C:\Users\Admin\source\repos\SchoolNewspaperBlazorApp\Images";

        public async Task<string> GetPreviewAsync(IBrowserFile file)
        {
            using var stream = file.OpenReadStream(5 * 1024 * 1024);
            using var memoryStream = new MemoryStream();

            await stream.CopyToAsync(memoryStream);

            byte[] bytes = memoryStream.ToArray();

            return $"data:{file.ContentType};base64,{Convert.ToBase64String(bytes)}";
        }

        public async Task UploadImage(IBrowserFile file)
        {
            Directory.CreateDirectory(uploadPath);

            string extension = Path.GetExtension(file.Name);
            string fileName = $"{Guid.NewGuid()}{extension}";
            string fullPath = Path.Combine(uploadPath, fileName);

            using var stream = file.OpenReadStream(5 * 1024 * 1024);

            using var fileStream = new FileStream(
                fullPath,
                FileMode.Create
            );

            await stream.CopyToAsync(fileStream);

            int id = await fileRepository.GetLastFieldId();

            MediaFile attachment = new MediaFile
            {
                Id = id,
                FileName = fileName,
                FileType = extension
            };

            await fileRepository.AddFileAsync(attachment);
        }
    }
}