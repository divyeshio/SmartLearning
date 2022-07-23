using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Web.Endpoints.StandardEndpoints;

public class UpdateStandardRequest
{
    public const string Route = "/standards";
    [Required]
    public int Id { get; set; }

    [Range(1, 12)]
    [Required]
    public int Level { get; set; }
}
