using AutoMapper;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Web.DTO;

namespace SmartLearning.Web.Mappings
{
  public class GroupProfile : Profile
  {
    public GroupProfile()
    {
      CreateMap<Classroom, ClassViewModel>();
      CreateMap<ClassViewModel, Classroom>();
    }
  }
}
