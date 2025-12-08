using MediatR;
using ProductService.Domain.Contracts;

namespace ProductService.Application.UseCases.Qna.Questions.Commands.UpdateQuestion;

public class UpdateQuestionCommandHandler(IProductRepository productRepository) : IRequestHandler<UpdateQuestionCommand>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId) ?? throw new Exception("Product not found.");
        var question = (product.Qna?.Questions?.FirstOrDefault(q => q.QuestionId == request.QuestionId)) ?? throw new InvalidOperationException($"Question with ID {request.QuestionId} not found.");
        product.UpdateQuestion(question.QuestionId, request.UserId, request.Text);

        await _productRepository.UpdateQnaAsync(product.ProductId, question.QuestionId, null, question);
    }
}