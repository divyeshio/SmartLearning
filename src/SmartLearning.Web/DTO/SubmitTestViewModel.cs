using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Web.DTO
{
  public class SubmitTestViewModel
  {
    [Required]
    public long TestId { get; set; }
    [Required]
    public List<SubmitAnswersViewModel> Questions { get; set; }

    [Required]
    public int TestAttemptId { get; set; }
  }
}
