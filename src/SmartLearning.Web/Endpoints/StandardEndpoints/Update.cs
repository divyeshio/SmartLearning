using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.StandardAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.StandardEndpoints;

public class Update : EndpointBaseAsync
    .WithRequest<UpdateStandardRequest>
    .WithActionResult<UpdateStandardResponse>
{
  private readonly IRepository<Standard> _repository;

  public Update(IRepository<Standard> repository)
  {
    _repository = repository;
  }

  [HttpPut(UpdateStandardRequest.Route)]
  [SwaggerOperation(
      Summary = "Updates a Standard",
      Description = "Updates a Standard with a longer description",
      OperationId = "Standards.Update",
      Tags = new[] { "StandardEndpoints" })
  ]
  public override async Task<ActionResult<UpdateStandardResponse>> HandleAsync(UpdateStandardRequest request,
      CancellationToken cancellationToken)
  {
    var existingStandard = await _repository.GetByIdAsync(request.Id);

    if (existingStandard == null)
    {
      return NotFound();
    }
    existingStandard.UpdateLevel(request.Level);

    await _repository.UpdateAsync(existingStandard);

    var response = new UpdateStandardResponse(
        standard: new StandardRecord(existingStandard.Id, existingStandard.Level, existingStandard.DisplayName)
    );
    return Ok(response);
  }
}
