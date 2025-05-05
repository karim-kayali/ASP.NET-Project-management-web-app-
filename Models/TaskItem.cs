using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdvancedFinalProject.Models
{
    public class TaskItem
    {
        [Key]
        public int TaskId { get; set; }

        
        public string TaskName { get; set; }

        public string? TaskDescription { get; set; }

        public string TaskPriority { get; set; }

        [Required]
        public string TaskStatus { get; set; }  

        public DateTime? TaskDueDate { get; set; }
 
        [Required]
        public int CreatorId { get; set; }
        public User Creator { get; set; }
 
        public int AssignedToId { get; set; }
        public User Assignee { get; set; }

        
        public int ProjectId { get; set; }
        public Project? project { get; set; }
    }
}
