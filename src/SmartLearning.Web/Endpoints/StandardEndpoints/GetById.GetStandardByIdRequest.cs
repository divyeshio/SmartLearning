
namespace SmartLearning.Web.Endpoints.StandardEndpoints;

public class GetStandardByIdRequest
{
  public const string Route = "/standards/{StandardId:int}";
  public static string BuildRoute(int standardId) => Route.Replace("{StandardId:int}", standardId.ToString());

  public int StandardId { get; set; }
}
