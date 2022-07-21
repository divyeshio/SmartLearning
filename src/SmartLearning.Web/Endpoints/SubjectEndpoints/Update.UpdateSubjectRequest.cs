using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Web.Endpoints.SubjectEndpoints;

public class UpdateSubjectRequest
{
  public const string Route = "/subjects";
  [Required]
  public int Id { get; set; }
  [Required]
  public string? Name { get; set; }
}
