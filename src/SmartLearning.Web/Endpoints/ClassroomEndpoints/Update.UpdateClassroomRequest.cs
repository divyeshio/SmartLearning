using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Web.Endpoints.ClassroomEndpoints;

public class UpdateClassroomRequest
{
  public const string Route = "/classrooms";
  [Required]
  public int Id { get; set; }
  [Required]
  public int BoardId { get; set; }
  [Required]
  public int StandardId { get; set; }
  [Required]
  public int SubjectId { get; set; }
}
