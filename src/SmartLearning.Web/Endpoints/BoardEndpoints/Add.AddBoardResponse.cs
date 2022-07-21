namespace SmartLearning.Web.Endpoints.BoardEndpoints;

public class AddBoardResponse
{
  public AddBoardResponse(int id, string name, string abbrName)
  {
    Id = id;
    Name = name;
    AbbrName = abbrName;
  }
  public int Id { get; set; }
  public string Name { get; set; }
  public string AbbrName { get; set; }
}
