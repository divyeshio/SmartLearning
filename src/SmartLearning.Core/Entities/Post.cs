using System.ComponentModel.DataAnnotations;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities
{
  public class Post : EntityBase
  {
    [Key]
    public int Id { get; set; }

    [Required]
    [Url]
    public string Image { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [Required]
    public byte[] Content { get; set; }

    public string AuthorId { get; set; }

    public ApplicationUser Author { get; set; }

  }
}
