using AutoMapper;
using SmartLearning.Core.Entities.ClassAggregate;
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
