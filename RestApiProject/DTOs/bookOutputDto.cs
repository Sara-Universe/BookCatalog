using RestApiProject.Models;

namespace RestApiProject.DTOs
{
    public class BookOutputDto
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int PublishedYear { get; set; }
        public decimal Price { get; set; }
        public List<User> FavoritedByUsers { get; set; } = new List<User>();
        public List<User> WishlistedByUsers { get; set; } = new List<User>();
    }
}
