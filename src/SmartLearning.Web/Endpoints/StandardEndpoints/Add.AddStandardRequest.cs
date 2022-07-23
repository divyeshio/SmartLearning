using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Web.Endpoints.StandardEndpoints;

public class AddStandardRequest
{
    public const string Route = "/standards";
    [Required]
    [Range(1, 12)]
    public int Level { get; set; }
}
