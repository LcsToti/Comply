using System;
using System.Collections.Generic;
using System.Text;
using ProductService.Domain.Entities;
using ProductService.Domain.Enums;

namespace ProductService.Domain.Factories
{
    public class ProductFactory
    {
        public static Product Create(Guid sellerId, string title, string description, string locale, Dictionary<string, string> characteristics, ProductCondition condition,
            Categories category, DeliveryPreferences deliveryPreference)
        {
            if (sellerId == Guid.Empty)
            {
                throw new ArgumentException("SellerId cannot be empty.", nameof(sellerId));
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be empty.", nameof(title));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description), "Description cannot be null or empty.");

            }

            if (string.IsNullOrWhiteSpace(locale))
            {
                throw new ArgumentNullException(nameof(locale), "Locale cannot be null or empty.");
            }

            if (characteristics.Count > 50)
            {
                throw new ArgumentException("Characteristics cannot exceed 50 items.", nameof(characteristics));
            }

            foreach (var i in characteristics) 
            {
                if (string.IsNullOrWhiteSpace(i.Key) || string.IsNullOrWhiteSpace(i.Value))
                {
                    throw new ArgumentException("Characteristic keys and values cannot be empty or whitespace.", nameof(characteristics));
                }
            }

            if (condition.ToString() is null)
            {
                throw new ArgumentNullException(nameof(condition), "Condition cannot be null.");
            }

            if (condition != ProductCondition.New && condition != ProductCondition.Used &&
                condition != ProductCondition.NotWorking && condition != ProductCondition.Refurbished)
            {
                throw new ArgumentOutOfRangeException(nameof(condition), "Invalid product condition.");
            }

            if (category.ToString() is null)
            {
                throw new ArgumentNullException(nameof(category), "Category cannot be null.");
            }

            if (Enum.IsDefined(typeof(Categories), category) == false)
            {
                throw new ArgumentOutOfRangeException(nameof(category), "Invalid product category.");
            }

            if (deliveryPreference.ToString() is null)
            {
                throw new ArgumentNullException(nameof(deliveryPreference), "Delivery Preference cannot be null.");
            }


            return new Product(sellerId, title, description, locale, characteristics, condition, category, deliveryPreference);
        }

    }

}
