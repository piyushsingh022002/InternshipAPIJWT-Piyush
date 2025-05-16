using System.ComponentModel.DataAnnotations;

namespace InternshipAPI.Models
{
    public class LoginRequest
    {
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
    
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }
        public DateTime Expiration { get; set; }
        public string Message { get; set; }
    }
    
    public class RegisterUserRequest
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Password { get; set; }
        
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Role { get; set; }
    }
}
