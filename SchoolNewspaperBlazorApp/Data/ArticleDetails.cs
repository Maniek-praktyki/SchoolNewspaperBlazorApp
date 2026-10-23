namespace SchoolNewspaperBlazorApp.Data
{
    // Dodatkowe dane zależne od typu artykułu. Każdy artykuł ma co najwyżej jeden rekord.
    // Pola, których dany typ nie używa, pozostają puste.
    public class ArticleDetails
    {
        public int ArticleId { get; set; }
        public Article Article { get; set; } = null!;

        public DateTime? Date { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Name { get; set; }
        public string? ClassName { get; set; }
        public string? TeacherName { get; set; }
        public string? Location { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}
