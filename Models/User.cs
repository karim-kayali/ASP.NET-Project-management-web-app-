
      using System.ComponentModel.DataAnnotations;

namespace AdvancedFinalProject.Models
{


    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, StringLength(100)]
        public string UserName { get; set; }

        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
 
        public List<TaskItem> Tasks { get; set; } = new ();

        
        public List<Project> Projects { get; set; } = new();
    }

}
