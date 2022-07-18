using System.ComponentModel.DataAnnotations;
using SmartLearning.Core.Entities.ClassAggregate;

namespace SmartLearning.Core.Entities.Common
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
