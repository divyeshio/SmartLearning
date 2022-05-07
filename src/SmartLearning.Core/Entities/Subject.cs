using System.ComponentModel.DataAnnotations;

namespace SmartLearning.Models
{
  public class Subject
  {

    [Key]
    [Required]
    public long Id { get; set; }
    [Required]
    [Display(Name = "Subject Name")]
    [StringLength(15)]
    public string Name { get; set; }

    /*[Column(TypeName = "nvarchar(max)")]
    public string Details { get; set; }*/

  }
}

