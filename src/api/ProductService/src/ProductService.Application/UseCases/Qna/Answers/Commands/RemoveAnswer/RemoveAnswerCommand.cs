using MediatR;

namespace ProductService.Application.UseCases.Qna.Answers.Commands.RemoveAnswer;

public record RemoveAnswerCommand(
    Guid ProductId,
    Guid SellerId,
    Guid QuestionId) : IRequest;