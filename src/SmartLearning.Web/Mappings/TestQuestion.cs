using AutoMapper;
using SmartLearning.Core.Entities.TestAggregate;
using SmartLearning.ViewModels;

namespace SmartLearning.Mappings
{
  public class TestQuestionProfile : Profile
  {
    public TestQuestionProfile()
    {
      CreateMap<TestQuestion, TestQuestionViewModel>();
      CreateMap<TestQuestionViewModel, TestQuestion>();
    }
  }
}
