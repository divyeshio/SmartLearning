using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Models
{
  public class Board
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
