namespace SmartLearning.Web.Endpoints.SubjectEndpoints;

public class UpdateSubjectResponse
{
  public UpdateSubjectResponse(SubjectRecord subject)
  {
    Subject = subject;
  }
  public SubjectRecord Subject { get; set; }
}
