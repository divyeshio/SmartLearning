namespace SmartLearning.Web.DTO
{
  public class StudentViewModel
  {
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string ConnectionId { get; set; }
    public string LiveClassId { get; set; }
    public bool? isAccepted { get; set; } = null;
  }
}
