using SchoolNewspaperBlazorApp.Data;

namespace SchoolNewspaperBlazorApp.Data.Enums
{
    // Wartości są zapisywane w bazie jako liczby. Nie zmieniaj ich po wdrożeniu.
    public enum ArticleType
    {
        Announcements = 1,
        Substitute = 2,
        Events = 3,
        Competitions = 4,
        Admissions = 5,
        Exams = 6,
        StudentCouncil = 7
    }

    // Pola dodatkowe, które może mieć artykuł zależnie od typu
    public enum ArticleField
    {
        Date,
        EndDate,
        StartTime,
        EndTime,
        Name,
        ClassName,
        TeacherName,
        Location,
        AdditionalInfo
    }

    public sealed record FieldRule(ArticleField Field, string Label, bool Required);

    public static class ArticleTypeInfo
    {
        private static readonly Dictionary<ArticleType, string> Labels = new()
        {
            [ArticleType.Announcements] = "Ogłoszenia",
            [ArticleType.Substitute] = "Zastępstwa",
            [ArticleType.Events] = "Wydarzenia",
            [ArticleType.Competitions] = "Zawody",
            [ArticleType.Admissions] = "Nabór",
            [ArticleType.Exams] = "Egzaminy",
            [ArticleType.StudentCouncil] = "Samorząd uczniowski"
        };

        private static readonly Dictionary<ArticleType, IReadOnlyList<FieldRule>> Rules = new()
        {
            [ArticleType.Announcements] = Array.Empty<FieldRule>(),

            [ArticleType.Substitute] = new FieldRule[]
            {
                new(ArticleField.Date, "Data zastępstwa", true),
                new(ArticleField.StartTime, "Od godziny", true),
                new(ArticleField.EndTime, "Do godziny", false),
                new(ArticleField.ClassName, "Klasa", true),
                new(ArticleField.TeacherName, "Nauczyciel", true),
                new(ArticleField.Name, "Przedmiot", false),
                new(ArticleField.AdditionalInfo, "Uwagi", false)
            },

            [ArticleType.Events] = new FieldRule[]
            {
                new(ArticleField.Date, "Data wydarzenia", true),
                new(ArticleField.StartTime, "Godzina rozpoczęcia", true),
                new(ArticleField.EndTime, "Godzina zakończenia", false),
                new(ArticleField.Location, "Miejsce", true),
                new(ArticleField.AdditionalInfo, "Dodatkowe informacje", false)
            },

            [ArticleType.Competitions] = new FieldRule[]
            {
                new(ArticleField.Name, "Nazwa zawodów", true),
                new(ArticleField.Date, "Data zawodów", true),
                new(ArticleField.StartTime, "Godzina rozpoczęcia", false),
                new(ArticleField.Location, "Miejsce zawodów", true),
                new(ArticleField.AdditionalInfo, "Dodatkowe informacje", false)
            },

            [ArticleType.Admissions] = new FieldRule[]
            {
                new(ArticleField.Date, "Początek naboru", true),
                new(ArticleField.EndDate, "Koniec naboru", true),
                new(ArticleField.AdditionalInfo, "Wymagane dokumenty", false)
            },

            [ArticleType.Exams] = new FieldRule[]
            {
                new(ArticleField.Name, "Przedmiot egzaminu", true),
                new(ArticleField.Date, "Data egzaminu", true),
                new(ArticleField.StartTime, "Godzina rozpoczęcia", true),
                new(ArticleField.EndTime, "Godzina zakończenia", false),
                new(ArticleField.Location, "Sala", false),
                new(ArticleField.AdditionalInfo, "Dodatkowe informacje", false)
            },

            [ArticleType.StudentCouncil] = new FieldRule[]
            {
                new(ArticleField.Date, "Data posiedzenia", true),
                new(ArticleField.StartTime, "Godzina rozpoczęcia", true),
                new(ArticleField.Location, "Miejsce", true),
                new(ArticleField.AdditionalInfo, "Porządek obrad", false)
            }
        };

        public static string Label(ArticleType type) => Labels[type];

        public static IReadOnlyList<FieldRule> Fields(ArticleType type) => Rules[type];

        // Zwraca wartość pola w formie do wyświetlenia albo null, gdy pola nie ma
        public static string? FormatValue(ArticleField field, ArticleDetails? details)
        {
            if (details == null)
            {
                return null;
            }

            return field switch
            {
                ArticleField.Date => details.Date?.ToString("dd.MM.yyyy"),
                ArticleField.EndDate => details.EndDate?.ToString("dd.MM.yyyy"),
                ArticleField.StartTime => details.StartTime?.ToString(@"hh\:mm"),
                ArticleField.EndTime => details.EndTime?.ToString(@"hh\:mm"),
                ArticleField.Name => details.Name,
                ArticleField.ClassName => details.ClassName,
                ArticleField.TeacherName => details.TeacherName,
                ArticleField.Location => details.Location,
                ArticleField.AdditionalInfo => details.AdditionalInfo,
                _ => null
            };
        }

        // Zwraca komunikat o błędzie albo null, gdy dane są poprawne
        public static string? Validate(ArticleType type, ArticleDetails? details)
        {
            foreach (var rule in Fields(type))
            {
                if (rule.Required && string.IsNullOrWhiteSpace(FormatValue(rule.Field, details)))
                {
                    return $"Uzupełnij pole: {rule.Label}.";
                }
            }

            if (details?.Date != null && details.EndDate != null && details.EndDate < details.Date)
            {
                return "Data końca nie może być wcześniejsza niż data początku.";
            }

            if (details?.StartTime != null && details.EndTime != null && details.EndTime <= details.StartTime)
            {
                return "Godzina zakończenia musi być późniejsza niż godzina rozpoczęcia.";
            }

            return null;
        }
    }
}
