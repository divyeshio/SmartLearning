using System.ComponentModel.DataAnnotations;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.ClassroomAggregate
{
  public class ReferenceBook : EntityBase
  {
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
    public int ClassId { get; set; }
    public Classroom Class { get; set; }
  }
}
