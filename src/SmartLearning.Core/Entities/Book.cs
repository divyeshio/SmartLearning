using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Models
{
  public class Book
  {
    [Key]
    [Required]
    public long Id { get; set; }

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
