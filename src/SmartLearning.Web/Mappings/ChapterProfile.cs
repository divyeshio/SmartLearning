using AutoMapper;
using SmartLearning.Core.Entities.ClassAggregate;
using SmartLearning.ViewModels;

namespace SmartLearning.Mappings
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
