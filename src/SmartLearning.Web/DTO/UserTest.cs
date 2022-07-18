using SmartLearning.Core.Entities.TestAggregate;

namespace SmartLearning.Web.DTO
{
  public class UserTest
  {
    public Test Test { get; set; }

    public bool isAttempted { get; set; }

    public bool isCompleted { get; set; }
    public TestResult TestResult { get; set; }

  }
}
