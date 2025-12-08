using FluentAssertions;
using ListingService.Domain.ListingAggregate;

namespace ListingService.Domain.Tests.ListingAggregate;

public class ListingExtensionsTests
{
    [Theory]
    [InlineData(ListingStatus.Available, ListingStatus.Paused)]
    [InlineData(ListingStatus.Paused, ListingStatus.Available)]
    public void ToggleVisibilityStatuses_WhenCalled_ShouldReturnOppositeVisibilityStatus(ListingStatus initialStatus, ListingStatus expectedStatus)
    {
        // Arrange
        // Initial status is provided by the test parameters

        // Act
        var newStatus = initialStatus.ToggleVisibilityStatuses();

        // Assert
        newStatus.Should().Be(expectedStatus);
    }
}