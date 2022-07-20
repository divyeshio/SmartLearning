using System.ComponentModel.DataAnnotations;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.ClassroomAggregate
{
  public class Chapter : EntityBase
  {

    [Required]
    [Display(Name = "Serial No")]
    [Range(0, 99, ErrorMessage = "Please Enter upto 2 digits only")]
    public int SerialNo { get; set; }

    [Required]
    [Display(Name = "Chapter Name")]
    [StringLength(20), MinLength(3)]
    public string Name { get; set; }

    [Required]
    public int ClassId { get; set; }
    public Classroom Class { get; set; }
  }
}

