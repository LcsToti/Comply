using MediatR;
using ProductService.Domain.Contracts;

namespace ProductService.Application.UseCases.Qna.Questions.Commands.AddQuestion;

public class AddQuestionCommandHandler(IProductRepository productRepository) : IRequestHandler<AddQuestionCommand>
{
    private readonly IProductRepository _productRepository = productRepository;

    public async Task Handle(AddQuestionCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId) 
            ?? throw new Exception("Product not found.");

        product.AddQuestion(request.UserId, request.Text);

        await _productRepository.UpdateAsync(product);
    }
}