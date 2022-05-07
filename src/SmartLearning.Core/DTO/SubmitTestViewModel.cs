using System.ComponentModel.DataAnnotations;

namespace SmartLearning.ViewModels
{
  public class SubmitTestViewModel
  {
    [Required]
    public long TestId { get; set; }
    [Required]
    public List<SubmitAnswersViewModel> Questions { get; set; }

    [Required]
    public string TestAttemptId { get; set; }
  }
}
