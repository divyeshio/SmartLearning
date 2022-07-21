using System.ComponentModel.DataAnnotations;
using Ardalis.GuardClauses;
using SmartLearning.SharedKernel;
using SmartLearning.SharedKernel.Interfaces;

namespace SmartLearning.Core.Entities.SubjectAggregate;

public class Subject : EntityBase, IAggregateRoot
{

  [Required]
  [Display(Name = "Subject Name")]
  [StringLength(15)]
  public string Name { get; set; }

  public Subject(string name)
  {
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
  }

  public void UpdateName(string newName)
  {
    Name = Guard.Against.NullOrEmpty(newName, nameof(newName));
  }

}

