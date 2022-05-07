using SmartLearning.Models;

namespace SmartLearning.ViewModels
{
  public class TestInsightsViewModel
  {
    public List<TestResult> Results { get; set; }
    public Test Test { get; set; }
    public int TotalQuestions { get; set; }
  }
}
