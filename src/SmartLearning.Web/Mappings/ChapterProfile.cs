using AutoMapper;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Web.DTO;

namespace SmartLearning.Web.Mappings
{
  public class ChapterProfile : Profile
  {
    public ChapterProfile()
    {
      CreateMap<Chapter, Chapter>();
      CreateMap<ChapterViewModel, Chapter>();
    }
  }
}
