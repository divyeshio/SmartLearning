
namespace SmartLearning.Web.Endpoints.ClassroomEndpoints;

public class GetClassroomByIdResponse
{
    public GetClassroomByIdResponse(int id, string name
      //,List<UsersRecord> users
      )
    {
        Id = id;
        Name = name;
        //Users = users;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    //public List<UsersRecord> Users { get; set; } = new();
}
