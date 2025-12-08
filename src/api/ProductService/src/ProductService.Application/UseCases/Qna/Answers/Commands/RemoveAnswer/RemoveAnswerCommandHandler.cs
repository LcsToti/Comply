using MediatR;
using ProductService.Domain.Contracts;

namespace ProductService.Application.UseCases.Qna.Answers.Commands.RemoveAnswer;

public class RemoveQuestionCommandHandler(IProductRepository productRepository) : IRequestHandler<RemoveAnswerCommand>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task Handle(RemoveAnswerCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId)
            ?? throw new InvalidOperationException($"Product with ID {request.ProductId} not found.");

        if (request.SellerId != product.SellerId)
            throw new UnauthorizedAccessException("Only the seller can remove the answer.");

        var question = product.Qna?.Questions?.FirstOrDefault(q => q.QuestionId == request.QuestionId)
            ?? throw new InvalidOperationException($"Question with ID {request.QuestionId} not found.");

        if (product.SellerId != request.SellerId)
            throw new UnauthorizedAccessException("Only the seller can remove the answer.");

        product.RemoveAnswer(request.SellerId, request.QuestionId);

        await _productRepository.UpdateQnaAsync(product.ProductId, question.QuestionId, question.Answer, null);
    }
}