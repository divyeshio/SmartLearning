
namespace SmartLearning.Web.Endpoints.SubjectEndpoints;

public class GetSubjectByIdResponse
{
    public GetSubjectByIdResponse(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
}
