using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.Core.Entities.ClassroomAggregate;
using SmartLearning.Core.Entities.StandardAggregate;
using SmartLearning.Core.Entities.SubjectAggregate;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Infrastructure.Data;
using static SmartLearning.Core.Constants.ApiRoutes;

namespace SmartLearning.Web;

public static class SeedData
{
  /*public static readonly Project TestProject1 = new Project("Test Project", PriorityStatus.Backlog);
  public static readonly ToDoItem ToDoItem1 = new ToDoItem
  {
      Title = "Get Sample Working",
      Description = "Try to get the sample to build."
  };
  public static readonly ToDoItem ToDoItem2 = new ToDoItem
  {
      Title = "Review Solution",
      Description = "Review the different projects in the solution and how they relate to one another."
  };
  public static readonly ToDoItem ToDoItem3 = new ToDoItem
  {
      Title = "Run and Review Tests",
      Description = "Make sure all the tests run and review what they are doing."
  };*/

  public static readonly string[] Subjects = { "English", "Mathematics", "Physics", "Economics", "Chemistry" };
  public static readonly string[] Boards = { "ICSE", "CBSE"};
  public static readonly string[] Roles = { "Admin", "Student", "Faculty"};


  public static void Initialize(IServiceProvider serviceProvider)
  {
    using var dbContext = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>(), null);
    using var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    using var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    // Look for any TODO items.
    if (dbContext.Users.Any())
    {
      return;   // DB has been seeded
    }

    PopulateTestData(dbContext, roleManager, userManager);
  }
  public static void PopulateTestData(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
  {
    //Add Subjects
    foreach (var item in Subjects)
    {
      dbContext.Subjects.Add(new Subject(item));
    }
    //Add Boards
    foreach (var item in Boards)
    {
      dbContext.Boards.Add(new Board(item, item));
    }
    //Add Standards
    foreach (var value in Enumerable.Range(1, 12))
    {
      dbContext.Standards.Add(new Standard(value));
    };
    //Add Roles
    foreach (var value in Roles)
    {
      roleManager.CreateAsync(new IdentityRole(value)).Wait();
    };
    var subject = dbContext.Subjects.First();
    var standard = dbContext.Standards.Where(s => s.Id == 11).First();
    var board = dbContext.Boards.Where(b => b.AbbrName == "ICSE").First();
    var AdminUser = new ApplicationUser { FirstName = "Admin", LastName = "Smart Learning", UserName = "admin@smartlearning.com", Email = "admin@smartlearning.com", AdminApproved = true, AccountType = AccountTypeEnum.Admin, EmailConfirmed = true };
    var StudentUser = new ApplicationUser { FirstName = "Student", LastName = "Smart", UserName = "student@smartlearning.com", Email = "student@smartlearning.com", AccountType = AccountTypeEnum.Student, EmailConfirmed = true, Board = board, Standard = standard };

    userManager.CreateAsync(AdminUser, "Admin@123").Wait();
    userManager.AddToRoleAsync(AdminUser, "Admin").Wait();
    userManager.CreateAsync(StudentUser, "Student@123").Wait();
    userManager.AddToRoleAsync(StudentUser, "Student").Wait();
    var groups = new List<Classroom>();
    var standards = dbContext.Standards.ToList();
    var subjects = dbContext.Subjects.ToList();
    var boards = dbContext.Boards.ToList();
    foreach (var subject1 in subjects)
    {
      foreach (var standard1 in standards)
      {
        foreach (var board1 in boards)
        {
          var group = new Classroom { Name = Classroom.GenerateGroupName(board1.AbbrName, standard1.DisplayName, subject1.Name), Board = board1, Standard = standard1, Subject = subject1 };
          groups.Add(group);

        }

      }
    }
    dbContext.Classes.AddRange(groups);
    var groups1 = dbContext.Classes.Where(g => g.BoardId == StudentUser.BoardId && g.StandardId == StudentUser.StandardId).ToList();
    foreach (var group in groups1)
    {
      group.Users.Add(StudentUser);
    }
    dbContext.SaveChanges();
    }
}
