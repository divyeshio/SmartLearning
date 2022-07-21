using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.Common;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Infrastructure.Data;
using SmartLearning.Web.DTO;
using Classroom = SmartLearning.Core.Entities.ClassroomAggregate.Classroom;

namespace SmartLearning.Web.Hubs
{
  [Authorize]
  public class ChatHub : Hub<IChatHub>
  {
    public readonly static List<UserViewModel> _Connections = new List<UserViewModel>();
    private readonly static Dictionary<string, string> _ConnectionsMap = new();
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly IMapper _mapper;
    public ChatHub(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _mapper = mapper;
      _userManager = userManager;
    }

    public async Task<List<ClassViewModel>> GetGroups()
    {
      var user = await _userManager.GetUserAsync(Context.User);
      var groupsVM = new List<ClassViewModel>();
      if (user.AccountType == AccountTypeEnum.Admin)
      {
        foreach (var group in await _context.Classes.ToListAsync())
        {
          groupsVM.Add(_mapper.Map<Classroom, ClassViewModel>(group));
          await Groups.AddToGroupAsync(Context.ConnectionId, group.Name);
        }
      }
      else
      {
        foreach (var group in await _context.Users.Where(u => u.UserName == IdentityName).SelectMany(u => u.Classes).ToListAsync())
        {
          groupsVM.Add(_mapper.Map<Classroom, ClassViewModel>(group));
          await Groups.AddToGroupAsync(Context.ConnectionId, group.Name);
        }
      }
      return groupsVM;
    }

    public async Task SendMessage(int groupId, string message)
    {
      try
      {
        var user = _context.Users.Where(u => u.UserName == IdentityName).FirstOrDefault();
        var group = _context.Classes.Where(r => r.Id == groupId).FirstOrDefault();

        if (!string.IsNullOrEmpty(message.Trim()))
        {
          // Create and save message in database
          var msg = new Message()
          {
            Content = Regex.Replace(message, @"(?i)<(?!img|a|/a|/img).*?>", string.Empty),
            FromUser = user,
            ToClass = group,
            Timestamp = DateTime.Now
          };
          await _context.Messages.AddAsync(msg);
          await _context.SaveChangesAsync();

          // Broadcast the message
          var messageViewModel = _mapper.Map<Message, MessageViewModel>(msg);
          await Clients.Group(group.Name).NewMessage(messageViewModel);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        //await Clients.Caller.SendAsync("onError", "Message not send! Message should be 1-500 characters.");
      }
    }

    public IEnumerable<MessageViewModel> GetMessageHistory(int groupId)
    {
      var messageHistory = _context.Messages.Where(m => m.ToClassId == groupId)
              .Include(m => m.FromUser)
              .Include(m => m.ToClass)
              .OrderByDescending(m => m.Timestamp)
              .AsEnumerable()
              .Reverse()
              .ToList();

      return _mapper.Map<IEnumerable<Message>, IEnumerable<MessageViewModel>>(messageHistory);
    }

    public override Task OnConnectedAsync()
    {
      try
      {
        var user = _context.Users.Where(u => u.UserName == IdentityName).FirstOrDefault();
        var userViewModel = _mapper.Map<ApplicationUser, UserViewModel>(user);

        if (!_Connections.Any(u => u.Username == IdentityName))
        {
          _Connections.Add(userViewModel);
          _ConnectionsMap.Add(IdentityName, Context.ConnectionId);
        }

      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        //Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
      }
      return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
      try
      {
        var user = _Connections.Where(u => u.Username == IdentityName).First();
        _Connections.Remove(user);

        // Tell other users to remove you from their list
        //Clients.OthersInGroup(user.CurrentRoom).SendAsync("removeUser", user);

        // Remove mapping
        _ConnectionsMap.Remove(user.Username);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        //Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
      }

      return base.OnDisconnectedAsync(exception);
    }

    private string IdentityName
    {
      get { return Context.User.Identity.Name; }
    }

  }

  public interface IChatHub
  {
    Task GetGroups();
    Task SendMessage();
    Task NewMessage(MessageViewModel messageViewModel);
  }
}
