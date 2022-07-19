using SmartLearning.Core.Entities.TestAggregate;

namespace SmartLearning.Web.DTO
{
  public class StartTestViewModel
  {
    public Test Test { get; set; }

    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }

    public int AttemptId { get; set; }
  }
}
