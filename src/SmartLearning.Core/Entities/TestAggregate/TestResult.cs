using System.ComponentModel.DataAnnotations;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.TestAggregate
{
  public class TestResult : EntityBase
  {

    [Required]
    public int CorrectAnswers { get; set; }
    [Required]
    public int IncorrectAnswers { get; set; }
    [Required]
    public TimeSpan TimeTaken { get; set; }

    [Required]
    public int TestAttemptId { get; set; }
    public TestAttempt TestAttempt { get; set; }

  }
}
