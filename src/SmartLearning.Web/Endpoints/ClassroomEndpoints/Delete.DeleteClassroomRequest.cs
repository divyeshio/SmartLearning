﻿namespace SmartLearning.Web.Endpoints.ClassroomEndpoints;

public class DeleteClassroomRequest
{
  public const string Route = "/classrooms/{ClassroomId:int}";
  public static string BuildRoute(int classroomId) => Route.Replace("{ClassroomId:int}", classroomId.ToString());
  public int ClassroomId { get; set; }
}
