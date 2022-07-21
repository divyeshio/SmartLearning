using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Web.DTO
{
  public class TestViewModel
  {
    [Required]
    public int Id { get; set; }

    [Display(Name = "Chapter")]
    [Required]
    public int ChapterId { get; set; }

    [Required]
    [Range(5, 1000, ErrorMessage = "Test Duration must be more than 5 minutes")]
    [Display(Name = "Test Duration")]
    public int Minutes { get; set; }
    [Required]
    public int Hours { get; set; } = 0;

    [Required]
    [MinLength(2, ErrorMessage = "Minimum 10 Questions Required")]
    public List<TestQuestionViewModel> Questions { get; set; }
  }
}
