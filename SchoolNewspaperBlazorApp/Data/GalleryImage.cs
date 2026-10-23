namespace SchoolNewspaperBlazorApp.Data
{
    public class GalleryImage
    {
        // Id z tabeli Files; null dla podglądu, który nie jest jeszcze zapisany
        public int? FileId { get; set; }

        // Źródło obrazu: data:URL dla nowo dodanego pliku lub /Images/{nazwa} dla zapisanego
        public string PreviewSrc { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
    