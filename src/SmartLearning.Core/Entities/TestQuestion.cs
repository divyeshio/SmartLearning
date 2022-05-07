using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Models
{
  public class TestQuestion
  {
    [Key]
    [Required]
    public long Id { get; set; }
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

    public long TestId { get; set; }
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
