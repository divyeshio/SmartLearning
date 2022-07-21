using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace SmartLearning.Web.DTO.AccountViewModels
{
  public class LoginViewModel
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
    public List<AuthenticationScheme?>? ExternalLogins { get; set; }


    /*[EnumDataType(typeof(AccountTypeEnum))]
    public AccountTypeEnum AccountType { get; set; }*/
  }
}
