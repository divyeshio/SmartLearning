using System.ComponentModel.DataAnnotations;

namespace SmartLearning.ViewModels.AccountViewModels
{
  public class ForgotPasswordViewModel
  {
    [Required(ErrorMessage = "Email Id is required")]
    [EmailAddress]
    public string Email { get; set; }
  }
}
