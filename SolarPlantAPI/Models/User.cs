using System.ComponentModel.DataAnnotations;

namespace SolarPlantAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        public required string Username { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public required string PasswordHash { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }
        [Required]
        public string Role { get; set; } = "User";
    }
}
