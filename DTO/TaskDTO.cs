using System.ComponentModel.DataAnnotations;

namespace AdvancedFinalProject.DTO
{
    public class TaskDTO
    {
       
        public string TaskName { get; set; }

        public string? TaskDescription { get; set; }

        public string TaskPriority { get; set; }

        public DateTime? TaskDueDate { get; set; }

         
        public int AssignedToId { get; set; }


        
        public int CreatorId { get; set; }

        public int ProjectId { get; set; }
    }
}
