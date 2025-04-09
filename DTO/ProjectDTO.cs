using System.ComponentModel.DataAnnotations;

namespace AdvancedFinalProject.DTO
{
    public class ProjectDTO
    {

 

        [Required, StringLength(100)]
        public string ProjectTitle { get; set; }

        [StringLength(300)]
        public string? ProjectDescription { get; set; }

       
        public int CreatorId { get; set; }   
    }
}
