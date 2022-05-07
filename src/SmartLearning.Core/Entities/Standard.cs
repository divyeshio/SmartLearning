using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace SmartLearning.Models
{
  [Index(nameof(Name), IsUnique = true)]
  public class Standard
  {

    [Key]
    [Required]
    public string Id { get; set; } = System.Guid.NewGuid().ToString();

    [Required]
    [Display(Name = "Standard")]
    [Range(1, 99, ErrorMessage = "Please Enter upto 2 digits only")]
    public int Name { get; set; }

    [Required]
    [BindNever]
    [StringLength(2)]
    public string DisplayName { get; set; }

  }
}

