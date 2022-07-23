using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.StandardAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.StandardEndpoints;

public class List : EndpointBaseAsync
    .WithoutRequest
    .WithActionResult<StandardListResponse>
{
    private readonly IReadRepository<Standard> _repository;

    public List(IReadRepository<Standard> repository)
    {
        _repository = repository;
    }

    [HttpGet("/standards")]
    [SwaggerOperation(
        Summary = "Gets a list of all Standards",
        Description = "Gets a list of all Standards",
        OperationId = "Standard.List",
        Tags = new[] { "StandardEndpoints" })
    ]
    public override async Task<ActionResult<StandardListResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        var response = new StandardListResponse();
        response.Standards = (await _repository.ListAsync())
            .Select(standard => new StandardRecord(standard.Id, standard.Level, standard.DisplayName))
            .ToList();

        return Ok(response);
    }
}
