using System.ComponentModel.DataAnnotations;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.TestAggregate
{
  public class TestAttempt : EntityBase
  {
    public int? TestId { get; set; }
    public Test Test { get; set; }

    public string? StudentId { get; set; }
    public ApplicationUser? Student { get; set; }

    public bool isCompleted { get; set; } = false;

    [Required]
    public DateTime StartTime { get; set; }
  }
}
