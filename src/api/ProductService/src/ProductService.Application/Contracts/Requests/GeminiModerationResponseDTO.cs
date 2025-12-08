using System.Text.Json.Serialization;

namespace ProductService.Application.Contracts.Requests;

public class GeminiModerationResponseDTO
{
    [JsonPropertyName("moderation_passed")]
    public bool ModerationPassed { get; set; }

    [JsonPropertyName("text_rejection_reason")]
    public string TextRejectionReason { get; set; } = string.Empty;

    [JsonPropertyName("image_rejection_reason")]
    public string ImageRejectionReason { get; set; } = string.Empty;
}