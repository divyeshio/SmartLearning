using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Models
{
  public class TestAttempt
  {
    [Key]
    [Required]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public long? TestId { get; set; }
    public Test Test { get; set; }

    public string StudentId { get; set; }
    public ApplicationUser Student { get; set; }

    public bool isCompleted { get; set; } = false;

    [Required]
    public DateTime StartTime { get; set; }
  }
}
