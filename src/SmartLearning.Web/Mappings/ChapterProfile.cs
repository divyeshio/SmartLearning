using AutoMapper;
using SmartLearning.Models;
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
