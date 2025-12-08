using System.Security.Cryptography;
using SalesService.Domain.Aggregates.SaleAggregate.Enums;
using SalesService.Domain.Aggregates.SaleAggregate.Exceptions;
using SalesService.Domain.Common;

namespace SalesService.Domain.Aggregates.SaleAggregate.Entities;

public class Sale : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Guid SellerId { get; private set; }
    public Guid ListingId { get; private set; }
    public Guid PaymentId { get; private set; }
    public decimal ProductValue { get; private set; }
    public SaleStatus Status { get; private set; }
    public DeliveryStatus DeliveryStatus { get; private set; }
    public string? DeliveryCode { get; private set; }
    public bool IsDeliveryCodeUsed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Dispute? Dispute { get; private set; }

    internal Sale(Guid id, Guid productId, Guid buyerId, Guid sellerId, Guid listingId, Guid paymentId, 
        decimal productValue, DateTime createdAt, DateTime? updatedAt, SaleStatus status, DeliveryStatus deliveryStatus, 
        string? deliveryCode, bool isDeliveryCodeUsed, Dispute? dispute)
    {
        Id = id;
        ProductId = productId;
        BuyerId = buyerId;
        SellerId = sellerId;
        ListingId = listingId;
        PaymentId = paymentId;
        ProductValue = productValue;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
        Status = status;
        DeliveryStatus = deliveryStatus;
        DeliveryCode = deliveryCode;
        IsDeliveryCodeUsed = isDeliveryCodeUsed;
        Dispute = dispute;
    }

    #region SaleMethods

    /// <summary>
    /// Attempts to mark the sale as done by updating its status to <see cref="SaleStatus.Done"/>.
    /// If the sale has already been canceled, an exception is thrown.
    /// </summary>
    /// <exception cref="InvalidSaleException">
    /// Thrown when trying to mark a sale that has been canceled as done.
    /// </exception>
    public void MarkAsDone()
    {
        if (Status == SaleStatus.Cancelled)
            throw new InvalidSaleException("Sale cannot be marked as done because it was cancelled.");
                
        Status = SaleStatus.Done;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Attempts to mark the sale as canceled by updating its status to <see cref="SaleStatus.Cancelled"/>.
    /// If the sale has already been marked as done, an exception is thrown.
    /// </summary>
    /// <exception cref="InvalidSaleException">
    /// Thrown when trying to mark a sale that has already been completed as canceled.
    /// </exception>
    public void MarkAsCancelled()
    {
        if (Status == SaleStatus.Done)
            throw new InvalidSaleException("Sale cannot be marked as cancelled because it was already done.");
                
        Status = SaleStatus.Cancelled;   
        UpdatedAt = DateTime.UtcNow;
        }

    /// <summary>
    /// Attempts to mark the sale as awaiting delivery by updating its status to <see cref="SaleStatus.AwaitingDelivery"/>.
    /// If the sale has already been marked as done or canceled, an exception is thrown.
    /// </summary>
    /// <exception cref="InvalidSaleException">
    /// Thrown if the sale is already marked as done or canceled.
    /// </exception>
    public void MarkAsAwaitingDelivery()
    {
        if (Status == SaleStatus.Done)
                throw new InvalidSaleException("Sale cannot be marked as awaiting delivery because it was already done.");
        if (Status == SaleStatus.Cancelled)
                throw new InvalidSaleException("Sale cannot be marked as awaiting delivery because it was cancelled.");
                
        Status = SaleStatus.AwaitingDelivery;
        UpdatedAt = DateTime.UtcNow;   
        }

    /// <summary>
    /// Attempts to mark the sale as a dispute by updating its status to <see cref="SaleStatus.Dispute"/>.
    /// If the sale is already marked as done or canceled, an exception is thrown.
    /// </summary>
    /// <exception cref="InvalidSaleException">
    /// Thrown when trying to mark a sale as a dispute if the sale status is already <see cref="SaleStatus.Done"/> or <see cref="SaleStatus.Cancelled"/>.
    /// </exception>
    private void MarkAsDispute()
    {
        if (Status == SaleStatus.Done)
            throw new InvalidSaleException("Sale cannot be marked as dispute because it was already done.");
        if (Status == SaleStatus.Cancelled)
            throw new InvalidSaleException("Sale cannot be marked as dispute because it was cancelled.");
                
        Status = SaleStatus.Dispute;
        UpdatedAt = DateTime.UtcNow;
    }
    #endregion

    #region DisputeMethods

    /// <summary>
    /// Opens a dispute for the sale by providing a reason and changing the sale's status to <see cref="SaleStatus.Dispute"/>.
    /// An exception is thrown if a dispute is already associated with the sale.
    /// </summary>
    /// <param name="reason">The reason for opening the dispute.</param>
    /// <exception cref="InvalidSaleException">
    /// Thrown when a dispute already exists for the sale.
    /// </exception>
    public void OpenDispute(string reason)
    {
        if (Dispute != null)
            throw new InvalidSaleException("Sale already has a dispute.");
        
        Dispute = Dispute.Open(reason);
        MarkAsDispute();
    }

    /// <summary>
    /// Assigns an admin to the dispute associated with the sale.
    /// If the sale does not have an associated dispute, an exception is thrown.
    /// </summary>
    /// <param name="adminId">The unique identifier of the admin to assign to the dispute.</param>
    /// <exception cref="InvalidSaleException">
    /// Thrown when attempting to assign an admin to a sale without an existing dispute.
    /// </exception>
    public void AssignAdminToDispute(Guid adminId)
    {
        if (Dispute == null)
            throw new InvalidSaleException("Sale does not have a dispute.");
        
        Dispute.AssignAdmin(adminId);
    }

    #region DisputeClosingMethods

    /// <summary>
    /// Closes the dispute associated with the sale as solved, setting the resolution details.
    /// Throws an exception if the sale does not have an active dispute.
    /// </summary>
    /// <param name="resolution">The resolution details to be recorded when closing the dispute.</param>
    /// <param name="resolutionStatus"></param>
    /// <exception cref="InvalidSaleException">
    /// Thrown if the sale does not have an active dispute to be closed.
    /// </exception>
    public void CloseDisputeAs(string resolution, DisputeResolutionStatus resolutionStatus)
    {
        if (Dispute == null)
            throw new InvalidSaleException("Sale does not have a dispute.");

        Dispute.Close(resolutionStatus, resolution);
    }

    #endregion

    #endregion
    
    #region DeliveryMethods

    /// <summary>
    /// Generates a new delivery code for the sale and sets it to an unused state.
    /// The delivery code is a random numeric value formatted as a four-digit string.
    /// </summary>
    public void GenerateDeliveryCode()
    {
        int secureCode = RandomNumberGenerator.GetInt32(100000, 1000000);
        DeliveryCode = secureCode.ToString("D6");
        IsDeliveryCodeUsed = false;
    }

    /// <summary>
    /// Marks the sale as delivered by updating its delivery status to <see cref="DeliveryStatus.Delivered"/>.
    /// Validates the provided delivery code and ensures it has not been used previously.
    /// </summary>
    /// <param name="code">The delivery code to verify before marking the sale as delivered.</param>
    /// <exception cref="InvalidDeliveryException">
    /// Thrown if the sale is not in transit, the delivery code is invalid, or the code has already been used.
    /// </exception>
    public void MarkDeliveryAsDelivered(string code)
    {
        if (DeliveryStatus != DeliveryStatus.Shipped)
            throw new InvalidDeliveryException("Sale must be shipped to be marked as delivered.");

        if (IsDeliveryCodeUsed)
            throw new InvalidDeliveryException("Delivery code already used.");

        if (DeliveryCode != code)
            throw new InvalidDeliveryException("Invalid delivery code.");

        DeliveryStatus = DeliveryStatus.Delivered;
        IsDeliveryCodeUsed = true;
        DeliveryCode = null;
    }

    /// <summary>
    /// Marks the delivery status of the sale as <see cref="DeliveryStatus.Shipped"/>
    /// and updates the <see cref="Sale.UpdatedAt"/> timestamp.
    /// Throws an exception if the delivery status is already marked as <see cref="DeliveryStatus.Delivered"/>.
    /// </summary>
    /// <exception cref="InvalidDeliveryException">
    /// Thrown when attempting to mark the delivery as shipped if it is already marked as delivered.
    /// </exception>
    public void MarkDeliveryAsShipped()
    {
        if (DeliveryStatus == DeliveryStatus.Delivered)
            throw new InvalidDeliveryException("Sale cannot be marked as shipped because it was already delivered.");
        
        DeliveryStatus = DeliveryStatus.Shipped;
        UpdatedAt = DateTime.UtcNow;   
    }
    #endregion
}