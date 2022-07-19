using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Web.Endpoints.BoardEndpoints;

public class AddBoardRequest
{
  public const string Route = "/boards";
  [Required]
  public string? Name { get; set; }
  [Required]
  [MaxLength(10)]
  public string? AbbrName { get; set; }
}
