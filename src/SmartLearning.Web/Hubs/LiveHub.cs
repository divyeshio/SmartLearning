using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartLearning.Core.Entities.UsersAggregate;
using SmartLearning.Infrastructure.Data;
using SmartLearning.Web.DTO;

namespace SmartLearning.Web.Hubs;

public class LiveHub : Hub<ILiveClass>
{
  private readonly ApplicationDbContext _context;
  private readonly static Dictionary<string, string> _ConnectionsMap = new();
  private readonly static List<User> _Connections = new();
  private readonly List<FacultyViewModel> _Faculty;
  private readonly List<StudentViewModel> _Students;

  public LiveHub(ApplicationDbContext context, List<StudentViewModel> students, List<FacultyViewModel> faculties)
  {
    _context = context;
    _Students = students;
    _Faculty = faculties;
  }

  [Authorize(Roles = "Admin,Faculty")]
  public async Task JoinFaculty(string userId, int liveClassId)
  {
    if (userId != null && liveClassId != null)
    {
      var liveClass = await _context.LiveClasses.Where(lc => lc.Id == liveClassId).FirstOrDefaultAsync();
      if (liveClass != null)
      {
        var faculty = _Faculty.Where(u => u.UserId == userId).SingleOrDefault();
        if (faculty == null)
        {
          _Faculty.Add(new FacultyViewModel
          {
            UserName = IdentityName,
            ConnectionId = Context.ConnectionId,
            UserId = userId,
            LiveClassId = liveClass.Id,
          });
          await SendStudentListUpdate(userId, liveClass.Id);
          await Clients.Caller.JoinAcknowledgement("Success");
        }
        else
        {
          await SendStudentsUpdates(faculty.LiveClassId, LiveClassStatusEnum.Online);
          await SendStudentListUpdate(faculty.UserId, liveClass.Id);
          await Clients.Caller.JoinAcknowledgement("ReJoin");
        }
      }
    }
  }


  public async Task SendStudentsUpdates(int liveClassId, LiveClassStatusEnum status)
  {

    var list = _Students.Where(s => s.LiveClassId == liveClassId).ToList();
    var connectionList = new List<string>();
    list.ForEach(s => connectionList.Add(s.ConnectionId));
    await Clients.Clients(connectionList).LiveClassStatus(status.ToString());
  }

  [Authorize(Roles = "Admin,Student")]
  public async Task JoinClass(string userId, int liveClassId)
  {
    var liveClass = await _context.LiveClasses.Where(lc => lc.Id == liveClassId).FirstOrDefaultAsync();
    if (liveClass != null)
    {
      var faculty = _Connections.Find(f => f.Id == liveClass.BroadcasterId); //Check if Faculty is Connected or Not
      if (faculty == null)
      {
        await Clients.Caller.LiveClassStatus(LiveClassStatusEnum.Offline.ToString()); //Send 
        return;
      }
      var student = _Students.Where(u => u.UserId == userId && u.LiveClassId == liveClassId).SingleOrDefault();
      if (student != null)
      {
        if (student.isAccepted == false)
        {
          await Clients.Caller.LiveClassStatus(LiveClassStatusEnum.Rejected.ToString());
        }
        else if (student.isAccepted == true)
        {
          await Clients.Caller.LiveClassStatus(LiveClassStatusEnum.Approved.ToString());
          //await Clients.Caller.FacultyInfo(_Faculty.Where(u => u.LiveClassId == liveClassId).SingleOrDefault());
        }
      }
      else if (student == null)
      {
        _Students.Add(new StudentViewModel
        {
          UserName = IdentityName,
          ConnectionId = Context.ConnectionId,
          UserId = userId,
          LiveClassId = liveClass.Id,
        });
        await Clients.Caller.LiveClassStatus(LiveClassStatusEnum.Wait.ToString()); // Wait for Approval
      }
      if (faculty != null)
      {
        await SendStudentListUpdate(faculty.Id, liveClass.Id);
      }

    }
  }

  [Authorize(Roles = "Admin,Faculty")]
  public async Task Accept(string userId, string facultyId, int liveClassId)
  {
    _Students.Where(s => s.UserId == userId).ToList().ForEach(s => s.isAccepted = true);
    var Student = _Students.Where(s => s.UserId == userId).FirstOrDefault();
    var con = _ConnectionsMap.FirstOrDefault(c => c.Key == Student.UserId);
    await SendStudentListUpdate(facultyId, liveClassId);
    //await Clients.Client(con.Value).FacultyInfo(_Faculty.Where(u => u.LiveClassId == liveClassId).SingleOrDefault());
    await Clients.Client(con.Value).LiveClassStatus(LiveClassStatusEnum.Approved.ToString());
  }
  [Authorize(Roles = "Admin,Faculty")]
  public async Task Reject(string userId, string facultyId, int liveClassId)
  {
    _Students.Where(s => s.UserId == userId).ToList().ForEach(s => s.isAccepted = false);
    var Student = _Students.Where(s => s.UserId == userId).FirstOrDefault();
    var con = _ConnectionsMap.FirstOrDefault(c => c.Key == Student.UserId);
    await SendStudentListUpdate(facultyId, liveClassId);
    await Clients.Client(con.Value).LiveClassStatus(LiveClassStatusEnum.Rejected.ToString());
  }


  private async Task SendStudentListUpdate(string facultyId, int liveClassId)
  {
    var FacultyConnectionId = _ConnectionsMap.Where(c => c.Key == facultyId).FirstOrDefault();

    var studentList = _Students.Where(s => s.LiveClassId == liveClassId && s.isAccepted != false).ToList();

    await Clients.Client(FacultyConnectionId.Value).UpdateStudentList(studentList);
  }

  public async Task SendSignal(string signal, string targetConnectionId)
  {
    var signalingUser = _Connections.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
    var otherUser = _Connections.SingleOrDefault(u => u.Id == targetConnectionId);

    if (signalingUser == null || otherUser == null)
    {
      return;
    }

    // These folks are in a call together, let's let em talk WebRTC
    await Clients.Client(otherUser.ConnectionId).ReceiveSignal(signalingUser.Id, signal);

  }

  [Authorize(Roles = "Admin,Faculty")]
  public async Task StopLive(string broadcasterId, int liveClassId)
  {
    var liveClass = await _context.LiveClasses.Where(c => c.Id == liveClassId && c.BroadcasterId == broadcasterId).FirstOrDefaultAsync();
    if (liveClass != null)
    {
      await SendStudentsUpdates(liveClassId, LiveClassStatusEnum.Ended);
      await Clients.Caller.LiveClassStatus(LiveClassStatusEnum.Ended.ToString());
      _Students.RemoveAll(s => s.LiveClassId == liveClass.Id);
      _Faculty.RemoveAll(s => s.LiveClassId == liveClass.Id);
      _context.LiveClasses.Remove(liveClass);
      await _context.SaveChangesAsync();
    }

  }

  [Authorize]
  public override async Task OnConnectedAsync()
  {
    try
    {
      var user = await _context.Users.Where(u => u.UserName == IdentityName).FirstOrDefaultAsync();

      if (!_Connections.Any(u => u.Username == IdentityName))
      {
        _Connections.Add(new User { Id = user.Id, Username = user.UserName, ConnectionId = Context.ConnectionId });
        _ConnectionsMap.Add(user.Id, Context.ConnectionId);
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.ToString());
      //Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
    }
    await base.OnConnectedAsync();
  }

  [Authorize]
  public override async Task OnDisconnectedAsync(Exception exception)
  {
    try
    {
      var user = _Connections.Where(u => u.Username == IdentityName).First();
      _Connections.Remove(user);

      // Remove mapping
      _ConnectionsMap.Remove(user.Id);
      var student = _Students.Find(s => s.UserId == user.Id && s.isAccepted != false);
      if (student != null)
      {
        _Students.Remove(student);
        var live = await _context.LiveClasses.Where(l => l.Id == student.LiveClassId).FirstOrDefaultAsync();
        await SendStudentListUpdate(live.BroadcasterId, live.Id);
      }
      var faculty = _Faculty.Find(f => f.UserId == user.Id);
      if (faculty != null)
      {
        await SendStudentsUpdates(faculty.LiveClassId, LiveClassStatusEnum.Offline);
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine(ex.ToString());
      //Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
    }

    await base.OnDisconnectedAsync(exception);
  }




  private string IdentityName
  {
    get { return Context.User.Identity.Name; }
  }

}

public class User
{
  public string Username { get; set; }
  public string Id { get; set; }
  public AccountTypeEnum AccountType { get; set; }
  public string ConnectionId { get; set; }
}

public enum LiveClassStatusEnum
{
  Approved,
  Ended,
  Offline,
  Online,
  Rejected,
  Wait
}

public interface ILiveClass
{
  Task UpdateStudentList(List<StudentViewModel> studentList);
  Task FacultyInfo(FacultyViewModel vm);
  Task JoinAcknowledgement(string message);
  Task ReceiveSignal(string signalingUser, string signal);
  Task LiveClassStatus(string status);
}
