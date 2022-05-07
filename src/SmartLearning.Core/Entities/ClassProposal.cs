﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SmartLearning.Models
{
  public class ClassProposal
  {
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [BindRequired]
    [Display(Name = "Board")]
    public long BoardId { get; set; }
    public Board Board { get; set; }

    [Display(Name = "Standard")]
    public string StandardId { get; set; }
    public Standard Standard { get; set; }

    [Required]
    [Display(Name = "Subject")]
    public long SubjectId { get; set; }
    public Subject Subject { get; set; }

    public int Count { get; set; }
  }
}
