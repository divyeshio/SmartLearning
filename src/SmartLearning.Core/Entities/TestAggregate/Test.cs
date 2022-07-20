using System.ComponentModel.DataAnnotations;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.TestAggregate
{
  public class Test : EntityBase
  {

    [Display(Name = "Chapter")]
    public int ChapterId { get; set; }
    public Chapter Chapter { get; set; }

    public string CreatedById { get; set; }
    public ApplicationUser CreatedBy { get; set; }

    [Required]
    [DataType(DataType.Time)]
    [Display(Name = "Test Duration")]
    public TimeSpan TestDuration { get; set; }

    public ICollection<TestQuestion> Questions { get; set; }

  }
}
