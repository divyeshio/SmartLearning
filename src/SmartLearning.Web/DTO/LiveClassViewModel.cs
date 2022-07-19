
using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Web.DTO
{
  public class LiveClassViewModel
  {
    public string? StandardName { get; set; }
    public string? SubjectName { get; set; }
    public string? BoardName { get; set; }
    public string? ClassName { get; set; }

    [Required]
    public int ClassId { get; set; }
  }
}
