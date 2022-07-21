using Microsoft.AspNetCore.Mvc.Rendering;

namespace SmartLearning.Web.DTO
{
  public class AdminLiveClassViewModel
  {
    public string Standard { get; set; }
    public long? Subject { get; set; }
    public long? Board { get; set; }

    public SelectList Boards { get; set; }
    public SelectList Standards { get; set; }
    public SelectList Subjects { get; set; }
  }
}
