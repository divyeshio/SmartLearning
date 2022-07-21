namespace SmartLearning.Core.Interfaces
{
  public interface ISmsSender
  {
    Task SendSmsAsync(string number, string message);
  }
}
