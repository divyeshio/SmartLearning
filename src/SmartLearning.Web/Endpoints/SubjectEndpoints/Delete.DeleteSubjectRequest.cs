namespace SmartLearning.Web.Endpoints.SubjectEndpoints;

public class DeleteSubjectRequest
{
  public const string Route = "/subjects/{SubjectId:int}";
  public static string BuildRoute(int subjectId) => Route.Replace("{SubjectId:int}", subjectId.ToString());
  public int SubjectId { get; set; }
}
