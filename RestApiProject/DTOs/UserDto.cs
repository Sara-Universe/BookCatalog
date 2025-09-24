using System.ComponentModel.DataAnnotations;

namespace RestApiProject.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be 3–50 characters long.")]
        public string Username { get; set; }

        [Required]
        [RegularExpression("^(Admin|User)$", ErrorMessage = "Role must be either 'Admin' or 'User'.")]
        public string Role { get; set; } = "User";

        // Book IDs stored in CSV as "1;2;3"
        public List<int> FavoriteBookIds { get; set; } = new();
        public List<int> WishlistBookIds { get; set; } = new();
    }

}
