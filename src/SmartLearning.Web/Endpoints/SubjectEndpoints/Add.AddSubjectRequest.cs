using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Web.Endpoints.SubjectEndpoints;

public class AddSubjectRequest
{
  public const string Route = "/subjects";
  [Required]
  public string? Name { get; set; }
}
