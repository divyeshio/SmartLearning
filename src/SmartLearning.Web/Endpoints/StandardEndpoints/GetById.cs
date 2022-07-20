using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.StandardAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.StandardEndpoints;

public class GetById : EndpointBaseAsync
    .WithRequest<GetStandardByIdRequest>
    .WithActionResult<GetStandardByIdResponse>
{
  private readonly IRepository<Standard> _repository;

  public GetById(IRepository<Standard> repository)
  {
    _repository = repository;
  }

  [HttpGet(GetStandardByIdRequest.Route)]
  [SwaggerOperation(
      Summary = "Gets a single Standard",
      Description = "Gets a single Standard by Id",
      OperationId = "Standards.GetById",
      Tags = new[] { "StandardEndpoints" })
  ]
  public override async Task<ActionResult<GetStandardByIdResponse>> HandleAsync([FromRoute] GetStandardByIdRequest request,
      CancellationToken cancellationToken)
  {
    var entity = await _repository.GetByIdAsync(request.StandardId);
    if (entity == null) return NotFound();

    var response = new GetStandardByIdResponse
    (
        id: entity.Id,
        level: entity.Level,
        displayName: entity.DisplayName
    );
    return Ok(response);
  }
}
