using FluentAssertions;
using Payments.Domain.Aggregate.VOs;
using Payments.Domain.Exceptions.PaymentExceptions; // Importar exceção correta

namespace Payments.Domain.Tests.Aggregate.VOs;

public class TimestampsTests
{
    private readonly DateTime _now = DateTime.UtcNow;

    [Fact]
    public void Create_WithValidTimestamps_ShouldCreateSuccessfully()
    {
        // Arrange
        var createdAt = _now.AddMinutes(-10);
        var processedAt = _now.AddMinutes(-5);
        var withdrawnAt = _now.AddMinutes(-1);
        var updatedAt = _now;

        // Act
        var timestamps = Timestamps.Create(createdAt, processedAt, updatedAt, withdrawnAt);

        // Assert
        timestamps.CreatedAt.Should().Be(createdAt);
        timestamps.ProcessedAt.Should().Be(processedAt);
        timestamps.UpdatedAt.Should().Be(updatedAt);
        timestamps.WithdrawnAt.Should().Be(withdrawnAt);
    }

    [Fact]
    public void Create_WithNullOptionalDates_ShouldCreateSuccessfully()
    {
        // Arrange
        var createdAt = _now.AddMinutes(-10);
        var updatedAt = _now;

        // Act
        var timestamps = Timestamps.Create(createdAt, null, updatedAt, null);

        // Assert
        timestamps.CreatedAt.Should().Be(createdAt);
        timestamps.ProcessedAt.Should().BeNull();
        timestamps.UpdatedAt.Should().Be(updatedAt);
        timestamps.WithdrawnAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithCreatedAtInTheFuture_ShouldThrowInvalidPaymentParamsException()
    {
        // Arrange
        var futureDate = _now.AddMinutes(10);
        Action act = () => Timestamps.Create(futureDate, null, futureDate, null);

        // Assert
        // ATUALIZADO: Troca de ArgumentException para InvalidPaymentParamsException
        act.Should().Throw<InvalidPaymentParamsException>()
            .WithMessage("CreatedAt cannot be in the future.");
    }

    [Fact]
    public void Create_WithUpdatedAtEarlierThanCreatedAt_ShouldThrowInvalidPaymentParamsException()
    {
        // Arrange
        var createdAt = _now;
        var updatedAt = _now.AddMinutes(-1);
        Action act = () => Timestamps.Create(createdAt, null, updatedAt, null);

        // Assert
        // ATUALIZADO: Troca de ArgumentException para InvalidPaymentParamsException
        act.Should().Throw<InvalidPaymentParamsException>()
            .WithMessage("UpdatedAt cannot be earlier than CreatedAt.");
    }

    [Fact]
    public void Create_WithProcessedAtEarlierThanCreatedAt_ShouldThrowInvalidPaymentParamsException()
    {
        // Arrange
        var createdAt = _now;
        var processedAt = _now.AddMinutes(-1);
        Action act = () => Timestamps.Create(createdAt, processedAt, createdAt, null);

        // Assert
        // ATUALIZADO: Troca de ArgumentException para InvalidPaymentParamsException
        act.Should().Throw<InvalidPaymentParamsException>()
            .WithMessage("ProcessedAt cannot be earlier CreatedAt.");
    }

    [Fact]
    public void Create_WithUpdatedAtEarlierThanProcessedAt_ShouldThrowInvalidPaymentParamsException()
    {
        // Arrange
        var createdAt = _now.AddMinutes(-5);
        var processedAt = _now;
        var updatedAt = _now.AddMinutes(-1);
        Action act = () => Timestamps.Create(createdAt, processedAt, updatedAt, null);

        // Assert
        // ATUALIZADO: Troca de ArgumentException para InvalidPaymentParamsException
        act.Should().Throw<InvalidPaymentParamsException>()
            .WithMessage("UpdatedAt cannot be earlier than ProcessedAt.");
    }
    
    [Fact]
    public void Create_WithWithdrawnAtEarlierThanCreatedAt_ShouldThrowInvalidPaymentParamsException()
    {
        // Arrange
        var createdAt = _now;
        var withdrawnAt = _now.AddMinutes(-1);
        Action act = () => Timestamps.Create(createdAt, null, createdAt, withdrawnAt);

        // Assert
        // ATUALIZADO: Troca de ArgumentException para InvalidPaymentParamsException
        act.Should().Throw<InvalidPaymentParamsException>()
            .WithMessage("WithdrawnAt cannot be earlier than CreatedAt.");
    }

    [Fact]
    public void Create_WithWithdrawnAtEarlierThanProcessedAt_ShouldThrowInvalidPaymentParamsException()
    {
        // Arrange
        var createdAt = _now.AddMinutes(-10);
        var processedAt = _now.AddMinutes(-5);
        var withdrawnAt = _now.AddMinutes(-6);
        var updatedAt = _now;
        Action act = () => Timestamps.Create(createdAt, processedAt, updatedAt, withdrawnAt);

        // Assert
        // ATUALIZADO: Troca de ArgumentException para InvalidPaymentParamsException
        act.Should().Throw<InvalidPaymentParamsException>()
            .WithMessage("WithdrawnAt cannot be earlier than ProcessedAt.");
    }
}