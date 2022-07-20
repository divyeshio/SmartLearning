using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.Core.Entities.StandardAggregate;
using SmartLearning.Core.Entities.SubjectAggregate;

namespace SmartLearning.Web.DTO
{
  public class ChapterViewModel
  {
    [Required]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Serial No")]
    [Range(0, 99, ErrorMessage = "Please Enter upto 2 digits only")]
    public int SerialNo { get; set; } = 1;

    [Required]
    [Display(Name = "Chapter Name")]
    [StringLength(20), MinLength(3)]
    public string Name { get; set; }

    [Display(Name = "Board")]
    [Required]
    public int BoardId { get; set; }
    public Board Board { get; set; }

    [Display(Name = "Standard")]
    [Required]
    public int StandardId { get; set; }
    public Standard Standard { get; set; }

    [Display(Name = "Subject")]
    [Required]
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }

    public SelectList Boards { get; set; }
    public SelectList Standards { get; set; }
    public SelectList Subjects { get; set; }
  }
}
