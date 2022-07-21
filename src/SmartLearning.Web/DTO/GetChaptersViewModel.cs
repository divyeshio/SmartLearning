using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Web.DTO
{
  public class GetChaptersViewModel
  {
    [Required(ErrorMessage = "Standard is required")]
    public int Standard { get; set; }
    [Required(ErrorMessage = "Board is required")]
    public int Board { get; set; }
    [Required(ErrorMessage = "Subject is required")]
    public int Subject { get; set; }
  }
}
