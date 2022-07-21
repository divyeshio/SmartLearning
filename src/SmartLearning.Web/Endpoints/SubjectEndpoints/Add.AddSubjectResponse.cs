namespace SmartLearning.Web.Endpoints.SubjectEndpoints;

public class AddSubjectResponse
{
  public AddSubjectResponse(int id, string name)
  {
    Id = id;
    Name = name;
  }
  public int Id { get; set; }
  public string Name { get; set; }
}
