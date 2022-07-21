using System.ComponentModel.DataAnnotations;
using SmartLearning.Core.Entities.TestAggregate;

namespace SmartLearning.Web.DTO
{
  public class SubmitAnswersViewModel
  {
    public int Id { get; set; }

    [EnumDataType(typeof(AnswerOptions))]
    public AnswerOptions Answer { get; set; }

  }
}
