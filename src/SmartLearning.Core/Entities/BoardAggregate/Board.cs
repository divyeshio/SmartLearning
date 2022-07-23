using Ardalis.GuardClauses;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.SharedKernel;
using SmartLearning.SharedKernel.Interfaces;

namespace SmartLearning.Core.Entities.BoardAggregate;

public class Board : EntityBase, IAggregateRoot
{


    public string AbbrName { get; private set; }
    public string Name { get; private set; }

    private List<ApplicationUser> _users = new List<ApplicationUser>();
    public IEnumerable<ApplicationUser> Users => _users.AsReadOnly();


    public Board(string abbrName, string name)
    {
        Guard.Against.NullOrEmpty(abbrName, nameof(AbbrName));
        AbbrName = Guard.Against.InvalidInput(abbrName, nameof(abbrName), abbr => abbr.Length <= 10).ToUpperInvariant();
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
    }

    public void UpdateNameAndAbbreviation(string newAbbrName, string newName)
    {
        Guard.Against.NullOrEmpty(newAbbrName, nameof(newAbbrName));
        AbbrName = Guard.Against.InvalidInput(newAbbrName, nameof(newAbbrName), abbr => abbr.Length <= 10).ToUpperInvariant();
        Name = Guard.Against.NullOrEmpty(newName, nameof(newName));
    }
    //public Board(string abbrName, string name, ICollection<ApplicationUser> users)public ICollection<Class> Classes { get; set; }


}
