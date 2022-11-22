using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SmartLearning.Core.Entities.UsersAggregate;

namespace SmartLearning.Infrastructure.Data
{
  public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
  {
    public MyUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
      var identity = await base.GenerateClaimsAsync(user);
      if (user.AccountType == AccountTypeEnum.Faculty)
      {
        identity.AddClaim(new Claim("BoardId", user.BoardId.ToString(), ClaimValueTypes.Integer64));
        identity.AddClaim(new Claim("StandardId", user.StandardId.ToString()));
        identity.AddClaim(new Claim("SubjectId", user.SubjectId.ToString(), ClaimValueTypes.Integer64));
      }
      else if (user.AccountType == AccountTypeEnum.Student)
      {
        identity.AddClaim(new Claim("BoardId", user.BoardId.ToString()));
        identity.AddClaim(new Claim("StandardId", user.StandardId.ToString()));
      }
      identity.AddClaim(new Claim("avatar", user.Avatar ?? "default.jpg"));
      identity.AddClaim(new Claim(ClaimTypes.Role, user.AccountType.ToString()));
      identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
      return identity;
    }
  }
}
