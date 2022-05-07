using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Models
{
  public class Message
  {
    [Key]
    public int Id { get; set; }
    [Required]
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    [Required]
    public string FromUserId { get; set; }
    public ApplicationUser FromUser { get; set; }

    [Required]
    public string ToClassId { get; set; }
    public Class ToClass { get; set; }
  }
}
