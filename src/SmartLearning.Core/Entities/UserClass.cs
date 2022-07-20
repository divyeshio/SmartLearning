using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Core.Entities.UsersAggregate;

namespace SmartLearning.Core.Entities
{
  public class UserClass
  {
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public int ClassId { get; set; }
    public Classroom Class { get; set; }
  }
}
