using MediatR;
using ProductService.Domain.Contracts;

namespace ProductService.Application.UseCases.Qna.Answers.Commands.AddAnswer;

public class AddAnswerToQuestionCommandHandler(IProductRepository productRepository) : IRequestHandler<AddAnswerCommand>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task Handle(AddAnswerCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId)
            ?? throw new System.Exception("Product not found.");
        
        if (product.SellerId != request.SellerId)
            throw new UnauthorizedAccessException("Only the seller can add an answer.");

        var question = product.Qna.Questions!.FirstOrDefault(q => q.QuestionId == request.QuestionId); // Todo: Cuidado com esse "!"

        if (question is not null)
        {
            product.AnswerQuestion(request.QuestionId, request.SellerId, request.AnswerText);

            await _productRepository.UpdateQnaAsync(product.ProductId, question.QuestionId, question.Answer, null);
        }
    }
}