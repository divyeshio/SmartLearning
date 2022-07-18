using System.ComponentModel.DataAnnotations;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities
{
  public class Board : BaseEntity
  {
    [Key]
    [Required]
    public long Id { get; set; }

    [Required]
    [Display(Name = "Board Name")]
    [StringLength(10)]
    public string Name { get; set; }

    public ICollection<ApplicationUser> Users { get; set; }
    //public ICollection<Class> Classes { get; set; }
  }
}
