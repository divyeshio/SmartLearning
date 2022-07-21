using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Web.Endpoints.BoardEndpoints;

public class UpdateBoardRequest
{
  public const string Route = "/boards";
  [Required]
  public int Id { get; set; }
  [Required]
  public string? Name { get; set; }

  [MaxLength(10)]
  [Required]
  public string? AbbrName { get; set; }
}
