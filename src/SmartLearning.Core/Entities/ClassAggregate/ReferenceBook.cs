using System.ComponentModel.DataAnnotations;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.ClassAggregate
{
  public class ReferenceBook : EntityBase
  {
    [Key]
    [Required]
    public long Id { get; set; }

    [StringLength(200, MinimumLength = 3)]
    public string FileName { get; set; }
    public string FileUrl { get; set; }

    [StringLength(200, MinimumLength = 3)]
    public string ImageName { get; set; }

    [Required]
    [StringLength(25, MinimumLength = 3)]
    public string BookName { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Price { get; set; }


    [Required]
    [StringLength(25, MinimumLength = 3)]
    public string AuthorName { get; set; }

    [Required]
    public string ClassId { get; set; }
    public Class Class { get; set; }
  }
}
