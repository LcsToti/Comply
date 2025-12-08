using Microsoft.AspNetCore.Http;
using ProductService.Domain.Entities;
using static ProductService.Application.Common.RejectReasonsEnum;

namespace ProductService.Application.Contracts;

public interface IAiService
{
    Task<string> AnalyzeProductAsync(
        Product product, 
        List<IFormFile>? images = null,
        bool? expectedPassed = null,
        TextRejectReason? textRejectReason = null,
        ImageRejectReason? imageRejectReason = null);
}