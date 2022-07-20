using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Web.Endpoints.ClassroomEndpoints;

public class AddClassroomRequest
{
  public const string Route = "/classrooms";
  [Required]
  public int BoardId { get; set; }
  [Required]
  public int StandardId { get; set; }
  [Required]
  public int SubjectId { get; set; }
}
