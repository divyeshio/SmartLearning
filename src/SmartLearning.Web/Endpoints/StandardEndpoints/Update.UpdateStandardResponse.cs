namespace SmartLearning.Web.Endpoints.StandardEndpoints;

public class UpdateStandardResponse
{
  public UpdateStandardResponse(StandardRecord standard)
  {
    Standard = standard;
  }
  public StandardRecord Standard { get; set; }
}
