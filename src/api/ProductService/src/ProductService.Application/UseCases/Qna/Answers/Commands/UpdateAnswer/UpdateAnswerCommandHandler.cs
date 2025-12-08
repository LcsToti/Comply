using MediatR;
using ProductService.Domain.Contracts;

namespace ProductService.Application.UseCases.Qna.Answers.Commands.UpdateAnswer;

public class UpdateAnswerCommandHandler(IProductRepository productRepository) : IRequestHandler<UpdateAnswerCommand>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task Handle(UpdateAnswerCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId)
            ?? throw new InvalidOperationException($"Product with ID {request.ProductId} not found.");

        if (request.SellerId != product.SellerId)
            throw new UnauthorizedAccessException("Only the seller can update the answer.");

        var question = product.Qna?.Questions?.FirstOrDefault(q => q.QuestionId == request.QuestionId)
            ?? throw new InvalidOperationException($"Question with ID {request.QuestionId} not found.");

        if (product.SellerId != request.SellerId)
            throw new UnauthorizedAccessException("Only the seller can update the answer.");

        product.UpdateAnswer(request.QuestionId, request.SellerId, request.NewAnswerText);
        await _productRepository.UpdateQnaAsync(product.ProductId, question.QuestionId, question.Answer, null);
    }
}