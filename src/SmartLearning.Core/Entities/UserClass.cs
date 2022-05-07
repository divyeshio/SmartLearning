namespace SmartLearning.Models
{
  public class UserClass
  {
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public string ClassId { get; set; }
    public Class Class { get; set; }
  }
}
