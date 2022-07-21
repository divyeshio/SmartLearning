namespace SmartLearning.Web.Endpoints.BoardEndpoints;

public class DeleteBoardRequest
{
  public const string Route = "/boards/{BoardId:int}";
  public static string BuildRoute(int boardId) => Route.Replace("{BoardId:int}", boardId.ToString());
  public int BoardId { get; set; }
}
