using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InternshipAPI.Models
{
    public class Intern
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        
        [StringLength(15)]
        public string Phone { get; set; }
        
        [Required]
        public DateTime JoiningDate { get; set; }
        
        [StringLength(100)]
        public string Department { get; set; }
        
        [StringLength(200)]
        public string Project { get; set; }
        
        [ForeignKey("User")]
        public int? UserId { get; set; }
        public User User { get; set; }
    }
}
