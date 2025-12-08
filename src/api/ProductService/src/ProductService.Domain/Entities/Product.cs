using ProductService.Domain.Entities.ValueObject.ReadModels;
using ProductService.Domain.Entities.ValueObjects;
using ProductService.Domain.Enums;

namespace ProductService.Domain.Entities;

public class Product
{
    public Guid ProductId { get; private set; }
    public Guid SellerId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Locale { get; private set; }
    public List<string> Images { get; private set; }
    public ListingReadModel? Listing { get; private set; }
    public Qna Qna { get; private set; }
    public Dictionary<string, string> Characteristics { get; private set; }
    public ProductCondition Condition { get; private set; }
    public Categories Category { get; private set; }
    public DeliveryPreferences DeliveryPreference { get; private set; }
    public int WatchListCount { get; private set; }
    public bool Featured { get; private set; }
    public DateTime? ExpirationFeatureDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    #region Product

    internal Product(Guid sellerId, string title, string description, string locale,
        Dictionary<string, string>? characteristics, ProductCondition condition,
        Categories category, DeliveryPreferences deliveryPreference)
    {
        ProductId = Guid.NewGuid();
        SellerId = sellerId;
        Title = title;
        Description = description;
        Locale = locale;
        Characteristics = characteristics ?? [];
        Condition = condition;
        Category = category;
        DeliveryPreference = deliveryPreference;
        Images = [];
        Qna = new Qna();
        WatchListCount = 0;
        Featured = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    #region ReadModels
    public void SetListingReadModel(ListingReadModel listingReadModel)
    {
        Listing = listingReadModel;
    }
    #endregion

    #region UpdateProduct

    public void UpdateTitle(Guid sellerId, string? title)
    {
        if (this.SellerId != sellerId)
        {
            throw new InvalidOperationException("Only the seller can update the product.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException($"Title cannot be empty, {title}");
        }

        ;

        Title = title;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(Guid sellerId, string? description)
    {
        if (this.SellerId != sellerId)
        {
            throw new InvalidOperationException("Only the seller can update the product.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException($"Description cannot be empty, {description}");
        }

        ;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLocale(Guid sellerId, string? locale)
    {
        if (this.SellerId != sellerId)
        {
            throw new InvalidOperationException("Only the seller can update the product.");
        }

        if (string.IsNullOrWhiteSpace(locale))
        {
            throw new ArgumentException($"Locale cannot be empty, {locale}");
        }

        ;
        ;
        Locale = locale;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCharacteristics(Guid sellerId, Dictionary<string, string>? characteristics)
    {
        if (this.SellerId != sellerId)
        {
            throw new InvalidOperationException("Only the seller can update the product.");
        }

        if (characteristics == null || characteristics.Count == 0)
        {
            throw new ArgumentException($"Characteristics must have at least one item, {characteristics}");
        }

        if (characteristics.Count > 50)
        {
            throw new ArgumentOutOfRangeException($"Characteristics cannot exceed 50 items, {characteristics}");
        }

        if (characteristics.Any(i => string.IsNullOrWhiteSpace(i.Key) || string.IsNullOrWhiteSpace(i.Value)))
        {
            throw new ArgumentException(
                $"Characteristic keys and values cannot be empty or whitespace, {characteristics}");
        }

        Characteristics = characteristics;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCondition(Guid sellerId, ProductCondition? condition)
    {
        if (this.SellerId != sellerId)
        {
            throw new InvalidOperationException("Only the seller can update the product.");
        }

        if (condition == null) return;

        if (!Enum.IsDefined(typeof(ProductCondition), condition))
        {
            throw new ArgumentOutOfRangeException(nameof(condition), "Invalid product condition.");
        }

        Condition = condition.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCategory(Guid sellerId, Categories? category)
    {
        if (this.SellerId != sellerId)
        {
            throw new InvalidOperationException("Only the seller can update the product.");
        }

        if (category == null) return;
        if (!Enum.IsDefined(typeof(Categories), category))
        {
            throw new ArgumentOutOfRangeException(nameof(category), "Invalid product category.");
        }

        Category = category.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDeliveryPreference(Guid sellerId, DeliveryPreferences? deliveryPreference)
    {
        if (this.SellerId != sellerId)
        {
            throw new InvalidOperationException("Only the seller can update the product.");
        }

        if (deliveryPreference == null) return;
        if (!Enum.IsDefined(typeof(DeliveryPreferences), deliveryPreference))
        {
            throw new ArgumentOutOfRangeException(nameof(deliveryPreference), "Invalid delivery preference.");
        }

        DeliveryPreference = deliveryPreference.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    #endregion

    #region Images

    private const int MaxImages = 8;

    public void AddImages(Guid sellerId, List<string> imagesUrls)
    {
        if (sellerId != this.SellerId)
        {
            throw new InvalidOperationException("Only the seller can add images.");
        }

        if (imagesUrls == null)
        {
            throw new ArgumentNullException(nameof(imagesUrls), "Image list cannot be null");
        }

        if ((Images.Count + imagesUrls.Count) > MaxImages)
        {
            throw new ArgumentOutOfRangeException($"A product cannot have more than {MaxImages} images.");
        }

        foreach (var url in imagesUrls)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("Image URL cannot be empty or null.", nameof(imagesUrls));
            }

            if (Images.Contains(url))
            {
                continue;
            }

            Images.Add(url);
        }
    }

    public void RemoveImage(Guid sellerId, List<string> imageUrls)
    {
        if (sellerId != this.SellerId)
        {
            throw new InvalidOperationException("Only the seller can remove images.");
        }

        if (imageUrls == null || imageUrls.Count == 0)
        {
            throw new ArgumentException("Image URLs list cannot be null or empty.", nameof(imageUrls));
        }

        foreach (var imgUrl in imageUrls)
        {
            if (!Images.Remove(imgUrl))
            {
                throw new InvalidOperationException($"Image not found: {imgUrl}");
            }
        }
    }

    public void ReorderImages(Guid sellerId, List<string> orderedImages)
    {
        if (this.SellerId != sellerId)
        {
            throw new InvalidOperationException("Only the seller can reorder images.");
        }

        if (orderedImages == null)
        {
            throw new ArgumentNullException(nameof(orderedImages));
        }

        if (Images.Count != orderedImages.Count || !Images.All(orderedImages.Contains))
        {
            throw new InvalidOperationException("The provided list of images must contain all of the existing images.");
        }

        Images = orderedImages;
        UpdatedAt = DateTime.UtcNow;
    }

    #endregion

    #endregion

    #region QnA

    public void AddQuestion(Guid userId, string questionText)
    {
        if (string.IsNullOrWhiteSpace(questionText))
        {
            throw new ArgumentException($"Question text cannot be empty, {questionText}");
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        }

        var newQuestion = new Question(userId, questionText);
        this.Qna.AddQuestion(newQuestion);
    }

    public void UpdateQuestion(Guid questionId, Guid userId, string newQuestionText)
    {
        var question = Qna.Questions?.FirstOrDefault(q => q.QuestionId == questionId) ??
                       throw new InvalidOperationException($"Question not found, {questionId}");
        if (question.UserId != userId)
        {
            throw new InvalidOperationException("You are not authorized to update this question.");
        }

        if (string.IsNullOrWhiteSpace(newQuestionText))
        {
            throw new ArgumentException($"Question text cannot be empty, {newQuestionText}");
        }

        question.UpdateQuestion(newQuestionText);
    }

    public void RemoveQuestion(Guid questionId, Guid userId)
    {
        var question = Qna.Questions?.FirstOrDefault(q => q.QuestionId == questionId) ??
                       throw new InvalidOperationException($"Question not found, {questionId}");
        if (question.UserId != userId)
        {
            throw new InvalidOperationException("You are not authorized to update this question.");
        }

        this.Qna.RemoveQuestion(question);
    }

    public void AnswerQuestion(Guid questionId, Guid sellerId, string answerText)
    {
        var question = Qna.Questions?.FirstOrDefault(q => q.QuestionId == questionId) ??
                       throw new InvalidOperationException($"Question not found, {questionId}");

        if (this.SellerId != sellerId)
        {
            throw new InvalidOperationException("Only the seller can answer questions.");
        }

        if (string.IsNullOrWhiteSpace(answerText))
        {
            throw new ArgumentException($"Answer text cannot be empty, {answerText}");
        }

        question.AddAnswer(answerText);
    }

    public void RemoveAnswer(Guid questionId, Guid sellerId)
    {
        if (this.SellerId != sellerId)
        {
            throw new InvalidOperationException("Only the seller can remove answers.");
        }

        var question = Qna.Questions?.FirstOrDefault(q => q.QuestionId == questionId) ??
                       throw new InvalidOperationException($"Question not found, {questionId}");

        question.RemoveAnswer();
    }

    public void UpdateAnswer(Guid questionId, Guid sellerId, string newAnswerText)
    {
        if (this.SellerId != sellerId)
        {
            throw new InvalidOperationException("Only the seller can update answers.");
        }

        var question = Qna.Questions?.FirstOrDefault(q => q.QuestionId == questionId) ??
                       throw new InvalidOperationException($"Question not found, {questionId}");

        if (question.Answer == null)
        {
            throw new InvalidOperationException($"Answer is null, {question.Answer}");
        }

        if (string.IsNullOrWhiteSpace(newAnswerText))
        {
            throw new ArgumentException($"Answer text cannot be empty, {newAnswerText}");
        }

        question.Answer.UpdateAnswer(newAnswerText);
    }

    #endregion

    #region Featured & Watchlist

    private const int MaxFeaturedDays = 30;

    public void AddFeatured(Guid sellerId, int days)
    {
        if (SellerId != sellerId)
            throw new InvalidOperationException("Only the seller can feature this product.");

        if (Featured)
            throw new InvalidOperationException("Product is already featured.");

        if (days is <= 0 or > MaxFeaturedDays)
            throw new ArgumentOutOfRangeException(nameof(days), $"Days must be between 1 and {MaxFeaturedDays}.");

        Featured = true;
        ExpirationFeatureDate = DateTime.UtcNow.AddDays(days);
    }


    public void ExtendFeatured(Guid sellerId, int days)
    {
        if (SellerId != sellerId)
            throw new InvalidOperationException("Only the seller can extend the featured period.");

        if (!Featured)
            throw new InvalidOperationException("Product is not featured.");

        if (days is <= 0 or > MaxFeaturedDays)
            throw new ArgumentOutOfRangeException(nameof(days), $"Days must be between 1 and {MaxFeaturedDays}.");

        if (ExpirationFeatureDate == null)
            throw new InvalidOperationException("Expiration date is not set.");

        var newExpirationDate = ExpirationFeatureDate.Value.AddDays(days);
        var totalDaysFromNow = (newExpirationDate - DateTime.UtcNow).TotalDays;

        if (totalDaysFromNow > MaxFeaturedDays)
            throw new InvalidOperationException($"Total featured days cannot exceed {MaxFeaturedDays} days from now.");

        ExpirationFeatureDate = newExpirationDate;
    }

    public void CheckExpiration()
    {
        if (!Featured)
        {
            throw new InvalidOperationException("Product is not featured.");
        }

        if (ExpirationFeatureDate == null)
        {
            throw new InvalidOperationException("Expiration date is not set.");
        }

        if (DateTime.UtcNow <= this.ExpirationFeatureDate.Value)
        {
            throw new InvalidOperationException("Product is not yet expired.");
        }

        Featured = false;
        ExpirationFeatureDate = null;
    }

    public void IncrementWatchList()
    {
        WatchListCount++;
    }

    public void DecrementWatchList()
    {
        if (WatchListCount > 0)
        {
            WatchListCount--;
        }
    }

    #endregion
}