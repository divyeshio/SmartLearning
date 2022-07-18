using System.ComponentModel.DataAnnotations;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities
{
  public class Subject : BaseEntity
  {

    [Key]
    [Required]
    public long Id { get; set; }
    [Required]
    [Display(Name = "Subject Name")]
    [StringLength(15)]
    public string Name { get; set; }

    /*[Column(TypeName = "nvarchar(max)")]
    public string Details { get; set; }*/

  }
}

