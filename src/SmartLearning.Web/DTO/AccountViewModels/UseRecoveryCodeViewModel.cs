using System.ComponentModel.DataAnnotations;

namespace SmartLearning.ViewModels.AccountViewModels
{
  public class UseRecoveryCodeViewModel
  {
    [Required]
    public string Code { get; set; }

    public string ReturnUrl { get; set; }
  }
}
