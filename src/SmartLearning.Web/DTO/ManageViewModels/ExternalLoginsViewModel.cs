using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace SmartLearning.Web.DTO.ManageViewModels
{
  public class ExternalLoginsViewModel
  {
    public IList<UserInfo> CurrentLogins { get; set; }

    public IList<AuthenticationScheme> OtherLogins { get; set; }

    public bool ShowRemoveButton { get; set; }
  }

  public class UserInfo
  {
    public string ProfileUrl { get; set; }

    public UserLoginInfo Info { get; set; }
  }
}
