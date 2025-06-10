// User represents an application user for authentication and authorization.
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; } // Primary key
        [Required]
        public string Username { get; set; } // Username for login
        [Required]
        public string PasswordHash { get; set; } // Hashed password
        public bool IsAdmin { get; set; } // Whether the user is an admin
        public bool IsApproved { get; set; } // Whether the user is approved
    }
}