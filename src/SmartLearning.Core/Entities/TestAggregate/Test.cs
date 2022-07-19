using System.ComponentModel.DataAnnotations;
using SmartLearning.Core.Entities.ClassAggregate;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.TestAggregate
{
  public class Test : EntityBase
  {
    [Key]
    [Required]

    public long Id { get; set; }

    [Display(Name = "Chapter")]
    public long ChapterId { get; set; }
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
