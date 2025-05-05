using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace AdvancedFinalProject.Models
  

{


    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required, StringLength(100)]
        public string ProjectTitle { get; set; }

        [StringLength(300)]
        public string? ProjectDescription { get; set; }

        [Required]
        
        public int CreatorId { get; set; }

        
        public User Creator { get; set; }

        

        public List<User> Members { get; set; } = new();


        public List<TaskItem> Tasks { get; set; } = new ();
    }
}
