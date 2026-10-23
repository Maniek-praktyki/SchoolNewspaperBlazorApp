using SchoolNewspaperBlazorApp.Data;
using SchoolNewspaperBlazorApp.Data.Enums;
using SchoolNewspaperBlazorApp.Interfaces.Repository;
using SchoolNewspaperBlazorApp.Interfaces.Service;

namespace SchoolNewspaperBlazorApp.Service
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;

        public ArticleService(
            IArticleRepository articleRepository,
            IPermissionService permissionService,
            INotificationService notificationService)
        {
            _articleRepository = articleRepository;
            _permissionService = permissionService;
            _notificationService = notificationService;
        }

        public async Task<int> AddArticleAsync(
            string title,
            string text,
            string author,
            int fileId,
            ArticleType category,
            ArticleDetails? details)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                _notificationService.Show("Tytuł nie może być pusty!", EnumNotificationType.Failure);
                return 0;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                _notificationService.Show("Treść nie może być pusta!", EnumNotificationType.Failure);
                return 0;
            }

            if (!Enum.IsDefined(category))
            {
                _notificationService.Show("Nieprawidłowy typ artykułu!", EnumNotificationType.Failure);
                return 0;
            }

            string? detailsError = ArticleTypeInfo.Validate(category, details);
            if (detailsError != null)
            {
                _notificationService.Show(detailsError, EnumNotificationType.Failure);
                return 0;
            }

            bool hasPermission = await _permissionService
                .HasPermissionAsync(EnumPermissions.AddArticle);

            if (!hasPermission)
            {
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return 0;
            }

            var article = new Article
            {
                Title = title,
                Text = text,
                Author = author,
                PublishDate = DateTime.Now,
                MainImageId = fileId,
                Category = category,
                Details = details
            };

            var articleId = await _articleRepository.AddArticleAsync(article);

            if (articleId != 0)
            {
                _notificationService.Show("Pomyślnie dodano artykuł!", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show(
                    "Podczas dodawania artykułu wystąpił błąd!",
                    EnumNotificationType.Error);
            }

            return articleId;
        }

        public async Task UpdateArticleByIdAsync(
            int id,
            string title,
            string text,
            string author,
            int fileId,
            ArticleType category,
            ArticleDetails? details)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                _notificationService.Show("Tytuł nie może być pusty!", EnumNotificationType.Failure);
                return;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                _notificationService.Show("Treść nie może być pusta!", EnumNotificationType.Failure);
                return;
            }

            if (!Enum.IsDefined(category))
            {
                _notificationService.Show("Nieprawidłowy typ artykułu!", EnumNotificationType.Failure);
                return;
            }

            string? detailsError = ArticleTypeInfo.Validate(category, details);
            if (detailsError != null)
            {
                _notificationService.Show(detailsError, EnumNotificationType.Failure);
                return;
            }

            bool hasPermission = await _permissionService
                .HasPermissionAsync(EnumPermissions.EditArticle);

            if (!hasPermission)
            {
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return;
            }

            var article = new Article
            {
                Id = id,
                Title = title,
                Text = text,
                Author = author,
                MainImageId = fileId,
                Category = category,
                Details = details
            };

            var success = await _articleRepository.UpdateArticleByIdAsync(article);

            if (success != 0)
            {
                _notificationService.Show("Pomyślnie zaktualizowano artykuł!", EnumNotificationType.Success);
            }
            else
            {
                _notificationService.Show(
                    "Podczas edytowania artykułu wystąpił błąd!",
                    EnumNotificationType.Error);
            }
        }

        public async Task<List<Article>> GetAllArticlesAsync()
        {
            return await _articleRepository.GetAllArticlesAsync();
        }

        public async Task<Article> GetArticleByIdAsync(int id)
        {
            Article article = await _articleRepository.GetArticleByIdAsync(id);

            if (article == null)
            {
                throw new Exception($"Article with ID {id} not found.");
            }

            return article;
        }

        public async Task<bool> RemoveArticleByIdAsync(int id)
        {
            bool hasPermission = await _permissionService
                .HasPermissionAsync(EnumPermissions.DeleteArticle);

            if (!hasPermission)
            {
                _notificationService.Show("Brak uprawnień!", EnumNotificationType.Failure);
                return false;
            }

            if (id <= 0)
            {
                _notificationService.Show("Nieprawidłowe ID artykułu!", EnumNotificationType.Failure);
                return false;
            }

            await _articleRepository.RemoveArticleByIdAsync(id);

            _notificationService.Show("Pomyślnie usunięto artykuł!", EnumNotificationType.Success);

            return true;
        }
    }
}
