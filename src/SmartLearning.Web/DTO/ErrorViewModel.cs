namespace SmartLearning.Web.DTO
{
  public class ErrorViewModel
  {
    public string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
  }
}
