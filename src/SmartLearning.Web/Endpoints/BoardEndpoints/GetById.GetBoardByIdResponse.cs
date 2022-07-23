
namespace SmartLearning.Web.Endpoints.BoardEndpoints;

public class GetBoardByIdResponse
{
    public GetBoardByIdResponse(int id, string abbrName, string name, List<UsersRecord> users)
    {
        Id = id;
        Name = name;
        AbbrName = abbrName;
        Users = users;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string AbbrName { get; set; }
    public List<UsersRecord> Users { get; set; } = new();
}
