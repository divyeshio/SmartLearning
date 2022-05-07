﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartLearning.Models;

namespace SmartLearning.ViewModels
{
  public class ChapterViewModel
  {
    [Required]
    public long Id { get; set; }

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
    public long BoardId { get; set; }
    public Board Board { get; set; }

    [Display(Name = "Standard")]
    [Required]
    public string StandardId { get; set; }
    public Standard Standard { get; set; }

    [Display(Name = "Subject")]
    [Required]
    public long SubjectId { get; set; }
    public Subject Subject { get; set; }

    public SelectList Boards { get; set; }
    public SelectList Standards { get; set; }
    public SelectList Subjects { get; set; }
  }
}
