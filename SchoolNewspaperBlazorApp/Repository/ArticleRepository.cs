using Microsoft.EntityFrameworkCore;
using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Interfaces.Repository;

namespace SchoolNewspaperBlazorApp.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IDbContextFactory<NewspaperDbContext> _contextFactory;

        public ArticleRepository(IDbContextFactory<NewspaperDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<int> AddArticleAsync(Article article)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            if (string.IsNullOrWhiteSpace(article.Title) ||
                string.IsNullOrWhiteSpace(article.Text))
            {
                return 0;
            }

            try
            {
                // Dodatkowe dane (article.Details) zapisują się razem z artykułem
                await context.Articles.AddAsync(article);
                await context.SaveChangesAsync();

                return article.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<int> UpdateArticleByIdAsync(Article article)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            if (string.IsNullOrWhiteSpace(article.Title) ||
                string.IsNullOrWhiteSpace(article.Text))
            {
                return 0;
            }

            try
            {
                var existingArticle = await context.Articles
                    .Include(a => a.Details)
                    .FirstOrDefaultAsync(a => a.Id == article.Id);

                if (existingArticle == null)
                {
                    return 0;
                }

                existingArticle.Title = article.Title;
                existingArticle.Text = article.Text;
                existingArticle.Author = article.Author;
                existingArticle.Category = article.Category;

                if (article.MainImageId != 0)
                {
                    existingArticle.MainImageId = article.MainImageId;
                }

                if (article.Details == null)
                {
                    // Nowy typ nie ma dodatkowych pól, więc usuwamy stary rekord
                    if (existingArticle.Details != null)
                    {
                        context.ArticleDetails.Remove(existingArticle.Details);
                        existingArticle.Details = null;
                    }
                }
                else if (existingArticle.Details == null)
                {
                    existingArticle.Details = new ArticleDetails
                    {
                        ArticleId = existingArticle.Id
                    };

                    CopyDetails(article.Details, existingArticle.Details);
                }
                else
                {
                    CopyDetails(article.Details, existingArticle.Details);
                }

                await context.SaveChangesAsync();

                return existingArticle.Id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Articles
                .Include(a => a.Details)
                .ToListAsync();
        }

        public async Task<Article> GetArticleByIdAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            var article = await context.Articles
                .Include(a => a.Details)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                throw new Exception($"Article with ID {id} not found.");
            }

            return article;
        }

        public async Task<bool> RemoveArticleByIdAsync(int id)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            try
            {
                var article = await context.Articles
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (article == null)
                {
                    return false;
                }

                // Rekord ArticleDetails jest usuwany kaskadowo razem z artykułem
                context.Articles.Remove(article);
                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void CopyDetails(ArticleDetails source, ArticleDetails target)
        {
            target.Date = source.Date;
            target.EndDate = source.EndDate;
            target.StartTime = source.StartTime;
            target.EndTime = source.EndTime;
            target.Name = source.Name;
            target.ClassName = source.ClassName;
            target.TeacherName = source.TeacherName;
            target.Location = source.Location;
            target.AdditionalInfo = source.AdditionalInfo;
        }
    }
}
