using MediatR;

namespace ProductService.Application.UseCases.Qna.Questions.Commands.RemoveQuestion;

public record RemoveQuestionCommand(
    Guid ProductId,
    Guid QuestionId,
    Guid UserId) : IRequest;
