using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.TestAggregate
{
  public class TestQuestion : EntityBase
  {
    [Required]

    public string Question { get; set; }

    [Required]
    public string OptionA { get; set; }
    [Required]
    public string OptionB { get; set; }
    [Required]
    public string OptionC { get; set; }
    [Required]
    public string OptionD { get; set; }


    [Required]
    [EnumDataType(typeof(AnswerOptions))]
    public AnswerOptions Answer { get; set; }

    public int TestId { get; set; }
    public Test Test { get; set; }


  }
  [Flags]
  [DefaultValue(NoneSelected)]
  public enum AnswerOptions
  {
    NoneSelected,
    OptionA,
    OptionB,
    OptionC,
    OptionD,
  }


}
