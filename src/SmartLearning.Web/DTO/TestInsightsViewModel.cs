using SmartLearning.Core.Entities.TestAggregate;

namespace SmartLearning.Web.DTO
{
  public class TestInsightsViewModel
  {
    public List<TestResult> Results { get; set; }
    public Test Test { get; set; }
    public int TotalQuestions { get; set; }
  }
}
