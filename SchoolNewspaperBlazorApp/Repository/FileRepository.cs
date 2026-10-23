using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;

        public FileRepository(IDbContextFactory<NewspaperDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<int> GetLastFileId()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            var id = await _context.Files
                .OrderByDescending(x => x.Id)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
            return id + 1;
        }

        public async Task<MediaFile> GetFileByIdAsync(int id)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            var file = await _context.Files.FirstOrDefaultAsync(a => a.Id == id);
            if (file == null)
            {
                throw new Exception("[FileRepository]Brak zdjecia w bazie danych");
            }
            return file;
        }

        public async Task<MediaFile?> FindFileByIdAsync(int id)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.Files.FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task AddFileAsync(MediaFile file)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            await _context.Files.AddAsync(file);
            await _context.SaveChangesAsync();
        }

        public async Task<List<MediaFile>> GetFilesByArticleIdAsync(int articleId)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            return await _context.Files
                .Where(f => f.ArticleId == articleId)
                .OrderBy(f => f.Id)
                .ToListAsync();
        }

        public async Task AttachFileToArticleAsync(int fileId, int articleId)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            var file = await _context.Files.FirstOrDefaultAsync(f => f.Id == fileId);
            if (file == null)
            {
                return;
            }

            file.ArticleId = articleId;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFileAsync(int id)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();

            var file = await _context.Files.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null)
            {
                return;
            }

            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
        }
    }
}
