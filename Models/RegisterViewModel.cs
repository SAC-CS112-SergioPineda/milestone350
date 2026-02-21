using System.ComponentModel.DataAnnotations;

namespace CST_350_MilestoneProject.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Sex { get; set; }

        [Required]
        [Range(1, 100)]
        public int Age { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
