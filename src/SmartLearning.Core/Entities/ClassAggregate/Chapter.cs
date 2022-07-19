using System.ComponentModel.DataAnnotations;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.ClassAggregate
{
  public class Chapter : EntityBase
  {

    [Key]
    [Required]
    public long Id { get; set; }

    [Required]
    [Display(Name = "Serial No")]
    [Range(0, 99, ErrorMessage = "Please Enter upto 2 digits only")]
    public int SerialNo { get; set; }

    [Required]
    [Display(Name = "Chapter Name")]
    [StringLength(20), MinLength(3)]
    public string Name { get; set; }

    [Required]
    public string ClassId { get; set; }
    public Class Class { get; set; }
  }
}

