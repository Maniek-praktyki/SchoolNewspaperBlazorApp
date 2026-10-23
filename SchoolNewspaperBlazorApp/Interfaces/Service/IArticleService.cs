using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;

namespace SchoolNewspaperBlazorApp.Interfaces.Service
{
    public interface IArticleService
    {
        Task<int> AddArticleAsync(
            string title,
            string text,
            string author,
            int fileId,
            ArticleType category,
            ArticleDetails? details);

        Task<List<Article>> GetAllArticlesAsync();

        Task<Article> GetArticleByIdAsync(int id);

        Task<bool> RemoveArticleByIdAsync(int id);

        Task UpdateArticleByIdAsync(
            int id,
            string title,
            string text,
            string author,
            int fileId,
            ArticleType category,
            ArticleDetails? details);
    }
}
