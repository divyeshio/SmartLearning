using SmartLearning.Models;

namespace SmartLearning.ViewModels
{
  public class StartTestViewModel
  {
    public Test Test { get; set; }

    public int Hours { get; set; }
    public int Minutes { get; set; }
    public int Seconds { get; set; }

    public string AttemptId { get; set; }
  }
}
