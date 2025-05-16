using System;
using System.ComponentModel.DataAnnotations;

namespace InternshipAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Password { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Email { get; set; }
        
        [Required]
        public string Role { get; set; } // "HR" or "Intern"
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
