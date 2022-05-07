using System.ComponentModel.DataAnnotations;
using SmartLearning.Models;

namespace SmartLearning.ViewModels
{
  public class SubmitAnswersViewModel
  {
    public long Id { get; set; }

    [EnumDataType(typeof(AnswerOptions))]
    public AnswerOptions Answer { get; set; }

  }
}
