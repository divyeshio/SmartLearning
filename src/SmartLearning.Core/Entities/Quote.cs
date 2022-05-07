using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Models
{
  public class Quote
  {
    [Key]

    public long Id { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 3)]
    public string AuthorName { get; set; } = "Unknown";

    [Required]
    [StringLength(1000), MinLength(3)]
    public string Details { get; set; }
  }
}

