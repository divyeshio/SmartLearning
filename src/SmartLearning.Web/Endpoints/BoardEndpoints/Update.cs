using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using SmartLearning.Core.Entities.BoardAggregate;
using SmartLearning.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartLearning.Web.Endpoints.BoardEndpoints;

public class Update : EndpointBaseAsync
    .WithRequest<UpdateBoardRequest>
    .WithActionResult<UpdateBoardResponse>
{
    private readonly IRepository<Board> _repository;

    public Update(IRepository<Board> repository)
    {
        _repository = repository;
    }

    [HttpPut(UpdateBoardRequest.Route)]
    [SwaggerOperation(
        Summary = "Updates a Board",
        Description = "Updates a Board with a longer description",
        OperationId = "Boards.Update",
        Tags = new[] { "BoardEndpoints" })
    ]
    public override async Task<ActionResult<UpdateBoardResponse>> HandleAsync(UpdateBoardRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Name == null || request.AbbrName == null)
        {
            return BadRequest();
        }
        var existingBoard = await _repository.GetByIdAsync(request.Id);

        if (existingBoard == null)
        {
            return NotFound();
        }
        existingBoard.UpdateNameAndAbbreviation(request.AbbrName, request.Name);

        await _repository.UpdateAsync(existingBoard);

        var response = new UpdateBoardResponse(
            board: new BoardRecord(existingBoard.Id, existingBoard.AbbrName, existingBoard.Name)
        );
        return Ok(response);
    }
}
