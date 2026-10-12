namespace SchoolNewspaperBlazorApp.Data
{
    public class User
    {
        public required int Id { get; set; }
        public required string Login { get; set; }
        public required string Haslo { get; set; }       
        public required string Email { get; set; }
        public required DateTime RegisterDate { get; set; }
        public string AddBy { get; set; } = string.Empty;
    }
}
