using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.BoardEndpoints;

public class Add : EndpointBaseAsync
    .WithRequest<AddBoardRequest>
    .WithActionResult<AddBoardResponse>
{
    private readonly IRepository<Board> _repository;

    public Add(IRepository<Board> repository)
    {
        _repository = repository;
    }

    [HttpPost(AddBoardRequest.Route)]
    [SwaggerOperation(
        Summary = "Adds a new Board",
        Description = "Adds a new Board",
        OperationId = "Boards.Add",
        Tags = new[] { "BoardEndpoints" })
    ]
    public override async Task<ActionResult<AddBoardResponse>> HandleAsync(AddBoardRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Name == null || request.AbbrName == null)
        {
            return BadRequest();
        }

        var newBoard = new Board(request.AbbrName, request.Name);

        var createdItem = await _repository.AddAsync(newBoard);

        var response = new AddBoardResponse
        (
            id: createdItem.Id,
            name: createdItem.Name,
            abbrName: createdItem.AbbrName
        );

        return Ok(response);
    }
}
