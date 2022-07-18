using System.ComponentModel.DataAnnotations;
using SmartLearning.Core.Entities.ClassAggregate;

namespace SmartLearning.Core.Entities.LiveClassAggregate
{
  public class LiveClass
  {
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string BroadcasterId { get; set; }

    public ApplicationUser Broadcaster { get; set; }

    public bool isAdmin { get; set; }

    public string ClassId { get; set; }

    public Class Class { get; set; }
  }
}
