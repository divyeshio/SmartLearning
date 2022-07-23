using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.StandardAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.StandardEndpoints;

public class Add : EndpointBaseAsync
    .WithRequest<AddStandardRequest>
    .WithActionResult<AddStandardResponse>
{
    private readonly IRepository<Standard> _repository;

    public Add(IRepository<Standard> repository)
    {
        _repository = repository;
    }

    [HttpPost(AddStandardRequest.Route)]
    [SwaggerOperation(
        Summary = "Adds a new Standard",
        Description = "Adds a new Standard",
        OperationId = "Standards.Add",
        Tags = new[] { "StandardEndpoints" })
    ]
    public override async Task<ActionResult<AddStandardResponse>> HandleAsync(AddStandardRequest request,
        CancellationToken cancellationToken)
    {

        var newStandard = new Standard(request.Level);

        var createdItem = await _repository.AddAsync(newStandard);

        var response = new AddStandardResponse
        (
            id: createdItem.Id,
            level: createdItem.Level,
            displayName: createdItem.DisplayName
        );

        return Ok(response);
    }
}
