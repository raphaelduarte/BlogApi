using System.ComponentModel.DataAnnotations;

namespace BlogApi.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "The email is mandatory")]
        [EmailAddress(ErrorMessage = "The format email is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password is mandatory")]
        public string Password { get; set; }
    }
}
