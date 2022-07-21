namespace SmartLearning.Web.Endpoints.ClassroomEndpoints;

public class AddClassroomResponse
{
  public AddClassroomResponse(int id, string name)
  {
    Id = id;
    Name = name;
  }
  public int Id { get; set; }
  public string Name { get; set; }
}
