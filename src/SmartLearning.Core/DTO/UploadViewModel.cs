using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SmartLearning.ViewModels
{
  public class UploadViewModel
  {
    [Required]
    public int RoomId { get; set; }
    [Required]
    public IFormFile File { get; set; }
  }
}
