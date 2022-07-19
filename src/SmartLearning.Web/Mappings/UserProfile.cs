using AutoMapper;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Web.DTO;

namespace SmartLearning.Web.Mappings
{
  public class UserProfile : Profile
  {
    public UserProfile()
    {
      CreateMap<ApplicationUser, UserViewModel>()
          .ForMember(dst => dst.Username, opt => opt.MapFrom(x => x.UserName));
      CreateMap<UserViewModel, ApplicationUser>();
    }
  }
}
