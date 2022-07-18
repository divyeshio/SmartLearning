using AutoMapper;
using SmartLearning.Core.Entities.ClassAggregate;
using SmartLearning.Web.DTO;

namespace SmartLearning.Web.Mappings
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
