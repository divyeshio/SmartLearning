using AutoMapper;
using SmartLearning.Models;
using SmartLearning.ViewModels;

namespace SmartLearning.Mappings
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
