using System.ComponentModel.DataAnnotations;
using SmartLearning.Core.Entities.TestAggregate;

namespace SmartLearning.ViewModels
{
  public class SubmitAnswersViewModel
  {
    public long Id { get; set; }

    [EnumDataType(typeof(AnswerOptions))]
    public AnswerOptions Answer { get; set; }

  }
}
