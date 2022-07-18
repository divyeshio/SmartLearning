using System.ComponentModel.DataAnnotations;

namespace SmartLearning.ViewModels
{
  public class GetChaptersViewModel
  {
    [Required(ErrorMessage = "Standard is required")]
    public string Standard { get; set; }
    [Required(ErrorMessage = "Board is required")]
    public long Board { get; set; }
    [Required(ErrorMessage = "Subject is required")]
    public long Subject { get; set; }
  }
}
