using System.ComponentModel.DataAnnotations;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.LiveClassAggregate
{
  public class LiveClass : EntityBase
  {

    [Required]
    public string? BroadcasterId { get; set; }

    public ApplicationUser? Broadcaster { get; set; }

    public bool isAdmin { get; set; }

    public int ClassId { get; set; }

    public Classroom Class { get; set; }
  }
}
