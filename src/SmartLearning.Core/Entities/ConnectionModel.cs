namespace SmartLearning.Core.Entities
{
  public class CallOffer
  {
    public User Caller { get; set; }
    public User Callee { get; set; }
  }

  public class User
  {
    public string Username { get; set; }
    public string ConnectionId { get; set; }
    public bool InCall { get; set; }
  }

  public class UserCall
  {
    public List<User> Users { get; set; }
  }
}
