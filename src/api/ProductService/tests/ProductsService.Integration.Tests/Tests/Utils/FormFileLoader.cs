using Microsoft.AspNetCore.Http;

namespace ProductsService.Integration.Tests.Tests.Utils;

public static class FormFileLoader
{
    public static IFormFile Load(string relativePath)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, relativePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");

        var fileBytes = File.ReadAllBytes(filePath);
        var stream = new MemoryStream(fileBytes);

        return new FormFile(stream, 0, stream.Length, "file", Path.GetFileName(relativePath));
    }
}
