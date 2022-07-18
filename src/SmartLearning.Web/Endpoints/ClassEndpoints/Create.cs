using Ardalis.ApiEndpoints;

namespace SmartLearning.Web.Endpoints.ClassEndpoints;

public class Create : EndpointBaseAsync
                      .WithRequest<CreateClassRequest>
                      .WithResult<CreateClassResponse>
{
  public override Task<CreateClassResponse> HandleAsync(CreateClassRequest request, CancellationToken cancellationToken = default)
  {
    throw new NotImplementedException();
  }
}
