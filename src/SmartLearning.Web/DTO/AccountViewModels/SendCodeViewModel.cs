using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartLearning.Web.DTO.AccountViewModels
{
  public class SendCodeViewModel
  {
    public string SelectedProvider { get; set; }

    public ICollection<SelectListItem> Providers { get; set; }

    public string ReturnUrl { get; set; }

    public bool RememberMe { get; set; }
  }
}
