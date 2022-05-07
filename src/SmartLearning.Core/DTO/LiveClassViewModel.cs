﻿
using System.ComponentModel.DataAnnotations;

namespace SmartLearning.ViewModels
{
  public class LiveClassViewModel
  {
    public string StandardName { get; set; }
    public string SubjectName { get; set; }
    public string BoardName { get; set; }
    public string ClassName { get; set; }

    [Required]
    public string ClassId { get; set; }
  }
}
