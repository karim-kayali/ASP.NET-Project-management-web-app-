using System.ComponentModel.DataAnnotations;

namespace AdvancedFinalProject.DTO
{
    public class EditTaskDTO
    {
         
        public string TaskName { get; set; }

        public string? TaskDescription { get; set; }

        public string TaskPriority { get; set; }

        public DateTime? TaskDueDate { get; set; }

    }
}
