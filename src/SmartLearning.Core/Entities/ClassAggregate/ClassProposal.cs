using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities.ClassAggregate
{
  public class ClassProposal : EntityBase
  {
    [BindRequired]
    [Display(Name = "Board")]
    public int BoardId { get; set; }
    public Board Board { get; set; }

    [Display(Name = "Standard")]
    public int StandardId { get; set; }
    public Standard Standard { get; set; }

    [Required]
    [Display(Name = "Subject")]
    public int SubjectId { get; set; }
    public Subject Subject { get; set; }

    public int Count { get; set; }
  }
}
