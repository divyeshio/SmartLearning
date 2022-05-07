using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Models
{
  public class TestResult
  {
    [Key]
    public long Id { get; set; }

    [Required]
    public int CorrectAnswers { get; set; }
    [Required]
    public int IncorrectAnswers { get; set; }
    [Required]
    public TimeSpan TimeTaken { get; set; }

    [Required]
    public string TestAttemptId { get; set; }
    public TestAttempt TestAttempt { get; set; }

  }
}
