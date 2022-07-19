﻿using System.ComponentModel.DataAnnotations;
using SmartLearning.SharedKernel;

namespace SmartLearning.Core.Entities
{
  public class Subject : EntityBase
  {

    [Required]
    [Display(Name = "Subject Name")]
    [StringLength(15)]
    public string Name { get; set; }

    /*[Column(TypeName = "nvarchar(max)")]
    public string Details { get; set; }*/

  }
}

