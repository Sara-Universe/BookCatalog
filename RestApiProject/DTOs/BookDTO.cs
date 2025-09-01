using System.ComponentModel.DataAnnotations;

namespace RestApiProject.DTOs
{
    public class BookDTO
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [StringLength(100, ErrorMessage = "Author name cannot be longer than 100 characters")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Genre is required")]
        [StringLength(30, ErrorMessage = "Genre cannot be longer than 30 characters")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "Published Year is required")]
        [Range(1200, 2025, ErrorMessage = "Published year must be between 1900 and 2025")]
        public int PublishedYear { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]

        public decimal Price { get; set; }
    }
}
