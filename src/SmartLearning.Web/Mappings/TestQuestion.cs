using AutoMapper;
using SmartLearning.Core.Entities.TestAggregate;
using SmartLearning.Web.DTO;

namespace SmartLearning.Web.Mappings
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
