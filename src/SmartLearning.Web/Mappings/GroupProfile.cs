using AutoMapper;
using SmartLearning.Models;
using SmartLearning.ViewModels;

namespace SmartLearning.Mappings
{
  public class GroupProfile : Profile
  {
    public GroupProfile()
    {
      CreateMap<Class, ClassViewModel>();
      CreateMap<ClassViewModel, Class>();
    }
  }
}
