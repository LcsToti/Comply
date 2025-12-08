using MediatR;

namespace ProductService.Application.UseCases.Qna.Answers.Commands.UpdateAnswer;

public record UpdateAnswerCommand(
    Guid ProductId,
    Guid QuestionId,
    Guid SellerId,
    string NewAnswerText) : IRequest;