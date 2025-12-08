using MediatR;

namespace ProductService.Application.UseCases.Qna.Answers.Commands.AddAnswer;

public record AddAnswerCommand(
    Guid ProductId,
    Guid QuestionId,
    Guid SellerId,
    string AnswerText) : IRequest;