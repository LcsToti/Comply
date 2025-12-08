using ProductsService.Domain.Tests.Commom;

namespace ProductsService.Domain.Tests;

public class QnaTests
{
    [Fact]
    public void AddQuestion_WithValidTextAndUser_ShouldAddQuestionToProduct()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        var userId = Guid.NewGuid();
        var questionText = "Este produto tem garantia?";

        // Act
        product.AddQuestion(userId, questionText);

        // Assert
        Assert.Single(product.Qna.Questions);
        Assert.Equal(questionText, product.Qna.Questions[0].QuestionText);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AddQuestion_WithNullOrEmptyText_ShouldThrowArgumentException(string invalidText)
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        var userId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => product.AddQuestion(userId, invalidText));
    }

    [Fact]
    public void AddQuestion_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => product.AddQuestion(Guid.Empty, "Texto válido?"));
    }

    [Fact]
    public void AnswerQuestion_WhenCalledBySeller_ShouldAddAnswer()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        product.AddQuestion(Guid.NewGuid(), "Funciona em 220V?");
        var questionId = product.Qna.Questions.First().QuestionId;
        var answerText = "Sim, funciona.";

        // Act
        product.AnswerQuestion(questionId, sellerId, answerText);

        // Assert
        Assert.NotNull(product.Qna.Questions[0].Answer);
        Assert.Equal(answerText, product.Qna.Questions[0].Answer.AnswerText);
    }

    [Fact]
    public void AnswerQuestion_WhenCalledByDifferentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        product.AddQuestion(Guid.NewGuid(), "Qual a cor?");
        var questionId = product.Qna.Questions.First().QuestionId;
        var differentUserId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            product.AnswerQuestion(questionId, differentUserId, "Resposta inválida.")
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AnswerQuestion_WithNullOrEmptyText_ShouldArgumentException(string invalidText)
    {
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        product.AddQuestion(Guid.NewGuid(), "Qual a cor?");
        var questionId = product.Qna.Questions.First().QuestionId;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => product.AnswerQuestion(questionId, sellerId, invalidText)
        );
    }

    [Fact]
    public void UpdateQuestion_WhenCalledByOwner_ShouldUpdateQuestionText()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        var questionOwnerId = Guid.NewGuid();
        product.AddQuestion(questionOwnerId, "Texto original?");
        var questionId = product.Qna.Questions.First().QuestionId;
        var newText = "Texto atualizado.";

        // Act
        product.UpdateQuestion(questionId, questionOwnerId, newText);

        // Assert
        Assert.Equal(newText, product.Qna.Questions.First().QuestionText);
    }

    [Fact]
    public void UpdateQuestion_WhenCalledByDifferentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        var questionOwnerId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        product.AddQuestion(questionOwnerId, "Texto original?");
        var questionId = product.Qna.Questions.First().QuestionId;

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            product.UpdateQuestion(questionId, differentUserId, "Novo texto.")
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateQuestion_WithNullOrEmptyText_ShouldArgumentException(string invalidText)
    {
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        product.AddQuestion(Guid.NewGuid(), "Qual a cor?");
        var questionId = product.Qna.Questions.First().QuestionId;
        var questionOwnerId = product.Qna.Questions.First().UserId;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => product.UpdateQuestion(questionId, questionOwnerId, invalidText)
        );

    }

    [Fact]
    public void UpdateAnswer_WhenCalledBySeller_ShouldUpdateAnswerText()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        var questionOwnerId = Guid.NewGuid();
        product.AddQuestion(questionOwnerId, "Texto original?");
        var questionId = product.Qna.Questions.First().QuestionId;
        product.AnswerQuestion(questionId, sellerId, "Resposta original.");
        var newAnswerText = "Resposta atualizada.";

        // Act
        product.UpdateAnswer(questionId, sellerId, newAnswerText);

        // Assert
        Assert.Equal(newAnswerText, product.Qna.Questions.First().Answer.AnswerText);
    }

    [Fact]
    public void UpdateAnswer_WhenCalledByDifferentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        var questionOwnerId = Guid.NewGuid();
        product.AddQuestion(questionOwnerId, "Texto original?");
        var questionId = product.Qna.Questions.First().QuestionId;
        product.AnswerQuestion(questionId, sellerId, "Resposta original.");
        var differentUserId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            product.UpdateAnswer(questionId, differentUserId, "Novo texto.")
        );
    }

    [Fact]
    public void UpdateAnswer_WhenAnswerNotExist_ShouldThrowInvalidOperationException()
    {
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        var questionOwnerId = Guid.NewGuid();
        product.AddQuestion(questionOwnerId, "Texto original?");
        var questionId = product.Qna.Questions.First().QuestionId;
        var newAnswerText = "Resposta atualizada.";

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            product.UpdateAnswer(questionId, sellerId, newAnswerText)
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateAnswer_WithTextNullOrEmpty_ShouldThrowArgumentException(string invalidText)
    {
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        var questionOwnerId = Guid.NewGuid();
        product.AddQuestion(questionOwnerId, "Texto original?");
        var questionId = product.Qna.Questions.First().QuestionId;
        product.AnswerQuestion(questionId, sellerId, "Resposta original.");
        var newAnswerText = invalidText;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => product.UpdateAnswer(questionId, sellerId, newAnswerText)
        );
    }

    [Fact]
    public void RemoveQuestion_WhenCalledByOwner_ShouldRemoveQuestion()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        var questionOwnerId = Guid.NewGuid();
        product.AddQuestion(questionOwnerId, "Texto original?");
        var questionId = product.Qna.Questions.First().QuestionId;
        // Act
        product.RemoveQuestion(questionId, questionOwnerId);
        // Assert
        Assert.Empty(product.Qna.Questions);
    }

    [Fact]
    public void RemoveQuestion_WhenCalledByDifferentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        var questionOwnerId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        product.AddQuestion(questionOwnerId, "Texto original?");
        var questionId = product.Qna.Questions.First().QuestionId;
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            product.RemoveQuestion(questionId, differentUserId)
        );
    }

    [Fact]
    public void RemoveAnswer_WhenCalledBySeller_ShouldRemoveAnswer()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        product.AddQuestion(Guid.NewGuid(), "Pergunta?");
        var questionId = product.Qna.Questions.First().QuestionId;
        product.AnswerQuestion(questionId, sellerId, "Resposta a ser removida.");

        // Act
        product.RemoveAnswer(questionId, sellerId);

        // Assert
        Assert.Null(product.Qna.Questions.First().Answer);
    }

    [Fact]
    public void RemoveAnswer_WhenCalledByDifferentUser_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var sellerId = Guid.NewGuid();
        var product = Common.CreateTestProduct(sellerId);
        var differentUserId = Guid.NewGuid();
        product.AddQuestion(Guid.NewGuid(), "Pergunta?");
        var questionId = product.Qna.Questions.First().QuestionId;
        product.AnswerQuestion(questionId, sellerId, "Resposta.");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            product.RemoveAnswer(questionId, differentUserId)
        );
    }
}