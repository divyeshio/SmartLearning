using System.ComponentModel.DataAnnotations;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Core.Entities.UsersAggregate;

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
    public int ToClassId { get; set; }
    public Classroom ToClass { get; set; }
  }
}
