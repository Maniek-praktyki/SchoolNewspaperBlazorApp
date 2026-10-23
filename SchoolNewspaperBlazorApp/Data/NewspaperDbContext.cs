using Microsoft.EntityFrameworkCore;

namespace SchoolNewspaperBlazorApp.Data
{
    public class NewspaperDbContext : DbContext
    {
        public NewspaperDbContext(DbContextOptions<NewspaperDbContext> options)
            : base(options) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleDetails> ArticleDetails { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<MediaFile> Files { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<MediaFile>()
                .Property(x => x.Id)
                .ValueGeneratedNever();

            mb.Entity<UserPermission>()
                .HasKey(up => new { up.UserId, up.PermissionId });

            mb.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(u => u.UserId);

            mb.Entity<UserPermission>()
                .HasOne(up => up.Permission)
                .WithMany(p => p.Permissions)
                .HasForeignKey(up => up.PermissionId);

            // Dodatkowe dane artykułu: klucz główny to ArticleId, czyli jeden rekord na artykuł
            mb.Entity<ArticleDetails>()
                .HasKey(d => d.ArticleId);

            mb.Entity<Article>()
                .HasOne(a => a.Details)
                .WithOne(d => d.Article)
                .HasForeignKey<ArticleDetails>(d => d.ArticleId);
        }
    }
}
