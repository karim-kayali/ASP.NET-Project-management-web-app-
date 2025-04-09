using System.ComponentModel.DataAnnotations;

namespace AdvancedFinalProject.DTO
{
    public class UserDTO
    {

        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}



