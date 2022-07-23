
namespace SmartLearning.Web.Endpoints.StandardEndpoints;

public class GetStandardByIdResponse
{
    public GetStandardByIdResponse(int id, int level, string displayName)
    {
        Id = id;
        Level = level;
        DisplayName = displayName;
    }

    public int Id { get; set; }
    public int Level { get; set; }
    public string DisplayName { get; set; }
}
