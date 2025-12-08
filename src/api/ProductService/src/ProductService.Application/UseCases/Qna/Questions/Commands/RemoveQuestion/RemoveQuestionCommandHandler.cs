using MediatR;
using ProductService.Domain.Contracts;

namespace ProductService.Application.UseCases.Qna.Questions.Commands.RemoveQuestion;

public class RemoveQuestionCommandHandler(IProductRepository productRepository) : IRequestHandler<RemoveQuestionCommand>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task Handle(RemoveQuestionCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId)
            ?? throw new KeyNotFoundException("Product not found.");

        var question = (product.Qna.Questions?.FirstOrDefault(q => q.QuestionId == request.QuestionId))
            ?? throw new KeyNotFoundException($"Question with ID '{request.QuestionId}' was not found.");

        if (request.UserId != question.UserId)
            throw new UnauthorizedAccessException("Only the owner of the question can remove it.");

        product.RemoveQuestion(request.QuestionId, request.UserId);

        await _productRepository.UpdateAsync(product);
    }
}