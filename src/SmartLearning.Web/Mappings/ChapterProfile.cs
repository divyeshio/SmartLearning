using AutoMapper;
using SmartLearning.Core.Entities.ClassAggregate;
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
