using System.ComponentModel.DataAnnotations;

namespace BlogApi.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "The name is mandatory")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "The email is mandatory")]
    [EmailAddress(ErrorMessage = "The mail is not valid")]
    public string Email { get; set; }
}