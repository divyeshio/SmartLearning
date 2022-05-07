using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Models
{
  public class LiveClass
  {
    [Key]
    public string Id { get; set; } = System.Guid.NewGuid().ToString();

    [Required]
    public string BroadcasterId { get; set; }

    public ApplicationUser Broadcaster { get; set; }

    public bool isAdmin { get; set; }

    public string ClassId { get; set; }

    public Class Class { get; set; }
  }
}
