namespace RestApiProject.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } 

        // New properties
        public List<int> FavoriteBookIds { get; set; } = new();
        public List<int> WishlistBookIds { get; set; } = new();
    }
}
