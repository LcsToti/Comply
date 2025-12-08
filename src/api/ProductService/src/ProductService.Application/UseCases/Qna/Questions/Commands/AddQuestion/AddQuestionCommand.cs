using MediatR;

namespace ProductService.Application.UseCases.Qna.Questions.Commands.AddQuestion
{
    public record AddQuestionCommand(Guid ProductId, Guid UserId, string Text) : IRequest; 
}
