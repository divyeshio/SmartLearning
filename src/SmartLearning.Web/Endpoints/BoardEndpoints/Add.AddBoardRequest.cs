using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Web.Endpoints.BoardEndpoints;

public class AddBoardRequest
{

  [Required]
  public string? Name { get; set; }
  [Required]
  public string? AbbrName { get; set; }
}
