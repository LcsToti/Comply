using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.UseCases.Qna.Answers.Commands.AddAnswer;
using ProductService.Application.UseCases.Qna.Answers.Commands.UpdateAnswer;
using ProductService.Application.UseCases.Qna.Questions.Commands.AddQuestion;
using ProductService.Application.UseCases.Qna.Questions.Commands.RemoveQuestion;
using ProductService.Application.UseCases.Qna.Questions.Commands.UpdateQuestion;
using System.Security.Claims;
using ProductService.API.Requests.Qna;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/v1/Products/{productId}/qna")]
[Authorize]
public class ProductQnaController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("questions")]
    public async Task<IActionResult> AddQuestion(Guid productId, [FromBody] QuestionRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();
        
        var command = new AddQuestionCommand(productId, userId, request.QuestionText);
        await _mediator.Send(command);

        return Ok();
    }

    [HttpPut("questions/{questionId}")]
    public async Task<IActionResult> UpdateQuestion(Guid productId, Guid questionId,
        [FromBody] QuestionRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();
        
        var command = new UpdateQuestionCommand(productId, questionId, userId, request.QuestionText);
        await _mediator.Send(command);
        return Accepted();
    }

    [HttpDelete("questions/{questionId}")]
    public async Task<IActionResult> RemoveQuestion(Guid productId, Guid questionId)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();
        
        var command = new RemoveQuestionCommand(productId, questionId, userId);
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("questions/{questionId}/answer")]
    public async Task<IActionResult> AnswerQuestion(Guid productId, Guid questionId, [FromBody] AnswerRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();
        
        var command = new AddAnswerCommand(productId, questionId, userId, request.AnswerText);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPut("questions/{questionId}/answer")]
    public async Task<IActionResult> UpdateAnswer(Guid productId, Guid questionId,
        [FromBody] AnswerRequest request)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();
        
        var command = new UpdateAnswerCommand(productId, questionId, userId, request.AnswerText);
        await _mediator.Send(command);
        return Accepted();
    }

    [HttpDelete("questions/{questionId}/answer")]
    public async Task<IActionResult> RemoveAnswer(Guid productId, Guid questionId,
        [FromBody] AnswerRequest request)
    {if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();
        
        var command = new UpdateAnswerCommand(productId, questionId, userId, string.Empty);
        await _mediator.Send(command);
        return NoContent();
    }
}