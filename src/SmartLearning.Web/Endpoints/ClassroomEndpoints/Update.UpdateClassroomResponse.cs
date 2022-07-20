namespace SmartLearning.Web.Endpoints.ClassroomEndpoints;

public class UpdateClassroomResponse
{
  public UpdateClassroomResponse(ClassroomRecord classroom)
  {
    Classroom = classroom;
  }
  public ClassroomRecord Classroom { get; set; }
}
