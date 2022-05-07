using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartLearning.ViewModels.AccountViewModels
{
  public class RegisterViewModel
  {
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    [Remote(action: "VerifyEmail", controller: "Users")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "*")]
    [Display(Name = "Standard")]
    public string StandardId { get; set; }

    [Required(ErrorMessage = "*")]
    [Display(Name = "Board")]
    public long? BoardId { get; set; }

    public Microsoft.AspNetCore.Mvc.Rendering.SelectList Boards { get; set; }
    public SelectList Standards { get; set; }
    public string ReturnUrl { get; set; }
    public List<AuthenticationScheme> ExternalLogins { get; set; }
  }
}
