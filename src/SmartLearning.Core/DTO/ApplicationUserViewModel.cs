﻿using System.ComponentModel.DataAnnotations;
using SmartLearning.Models;

namespace SmartLearning.ViewModels
{
  public class ApplicationUserViewModel
  {
    public string Id { get; set; }

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    [EmailAddress]
    [Required]
    public string Email { get; set; }
    [DataType(DataType.Password)]
    [Required]
    public string Password { get; set; }
    [EnumDataType(typeof(AccountTypeEnum))]
    public AccountTypeEnum AccountType { get; set; } = AccountTypeEnum.Admin;

    [Required]
    [Display(Name = "Standard")]
    public string StandardId { get; set; }
    public List<Standard> Standards { get; set; }
    [Required]
    [Display(Name = "Board")]
    public string BoardId { get; set; }
    public List<Board> Boards { get; set; }
    [Required]
    [Display(Name = "Subject")]
    public string SubjectId { get; set; }
    public List<Subject> Subjects { get; set; }
  }
}
