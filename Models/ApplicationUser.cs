using System.ComponentModel.DataAnnotations;

namespace KanbanBackend.Models
{
    // Ye class ek user ko represent karti hai (login/register ke liye)
    public class ApplicationUser
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Password kabhi bhi plain text me save nahi hoti — hamesha hash hoke save hoti hai
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Ek user ke multiple boards ho sakte hain (one-to-many relationship)
        public List<Board> Boards { get; set; } = new();
    }
}
