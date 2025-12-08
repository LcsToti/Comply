namespace ProductService.Application.Common;

public class RejectReasonsEnum
{
    public enum TextRejectReason
    {
        PROHIBITED_CONTENT,
        INVALID_RESPONSE,
        NONE
    }
    public enum ImageRejectReason
    {
        PROHIBITED_CONTENT,
        NO_PRODUCT_SHOWN,
        INVALID_RESPONSE,
        NONE
    }
}
