using SmartLearning.Core.Entities.ClassAggregate;
using SmartLearning.Core.Entities.UsersAggregate;

namespace SmartLearning.Core.Entities
{
  public class UserClass
  {
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string ClassId { get; set; }
    public Class Class { get; set; }
  }
}
