using MediatR;

namespace ProductService.Application.UseCases.Qna.Questions.Commands.UpdateQuestion;

public record UpdateQuestionCommand(
    Guid ProductId,
    Guid QuestionId,
    Guid UserId,
    string Text) : IRequest;