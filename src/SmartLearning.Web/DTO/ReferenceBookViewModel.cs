using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartLearning.Core.Helpers;

namespace SmartLearning.Web.DTO
{
  public class ReferenceBookViewModel
  {
    [Required]
    public int Id { get; set; }
    public SelectList Boards { get; set; }
    public SelectList Standards { get; set; }
    public SelectList Subjects { get; set; }
    public SelectList Classes { get; set; }

    [AllowedFileExtensions(new string[] { ".jpg", ".jpeg", ".gif", ".png" })]
    public IFormFile ImageFile { get; set; }

    [StringLength(200, MinimumLength = 3)]
    public string ImageName { get; set; } = "default.jpg";

    [AllowedFileExtensions(new string[] { ".pdf" })]
    public IFormFile File { get; set; }

    [Required(ErrorMessage = "Name is required"), DisplayName("Book Name")]
    [StringLength(25, MinimumLength = 3)]
    public string BookName { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid Price")]
    public int Price { get; set; }

    [Required, DisplayName("Author Name")]
    [StringLength(25, MinimumLength = 3)]
    public string AuthorName { get; set; }

    [Required]
    public int ClassId { get; set; }

    public string FileName { get; set; }
  }
}
