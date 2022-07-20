using System.ComponentModel.DataAnnotations;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.ClassroomAggregate
{
  public class Book : EntityBase
  {
    [Required]
    public string Title { get; set; }
    public int Price { get; set; }

    [Required]
    public string AuthorName { get; set; }

    [Required]
    public string PublisherName { get; set; }

    public int Year { get; set; }

    [Required]
    public string Details { get; set; }
    [Required]
    public string ImageUrl { get; set; }
  }
}
