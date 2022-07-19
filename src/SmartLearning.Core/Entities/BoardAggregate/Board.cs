using System.ComponentModel.DataAnnotations;
using Ardalis.GuardClauses;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.SharedKernel;
using SmartLearning.SharedKernel.Interfaces;

namespace SmartLearning.Core.Entities.BoardAggregate;

public class Board : EntityBase, IAggregateRoot
{

  [Display(Name = "Board Name")]
  public string AbbrName { get; private set; }
  public string Name { get; private set; }

  private List<ApplicationUser> _users = new List<ApplicationUser>();
  public IEnumerable<ApplicationUser> Users => _users.AsReadOnly();


  public Board(string abbrName, string name)
  {
    Guard.Against.InvalidInput(abbrName, nameof(abbrName), abbr => abbr.Length > 10);
    AbbrName = Guard.Against.NullOrEmpty(abbrName, nameof(abbrName)).ToUpperInvariant();
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
  }

  public void UpdateName(string newName)
  {
    Name = Guard.Against.NullOrEmpty(newName, nameof(newName));
  }
  //public Board(string abbrName, string name, ICollection<ApplicationUser> users)public ICollection<Class> Classes { get; set; }


}
