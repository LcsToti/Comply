using GenerativeAI;
using GenerativeAI.Types;
using Microsoft.AspNetCore.Http;
using ProductService.Application.Contracts;
using ProductService.Domain.Entities;
using System.Text.Json;
using static ProductService.Application.Common.RejectReasonsEnum;

namespace ProductService.Infrastructure.Services;

public class GeminiService : IAiService
{
    private readonly GenerativeModel _gemma;

    public GeminiService(string apiKey)
    {
        var gemmaAi = new GoogleAi(apiKey);
        _gemma = gemmaAi.CreateGenerativeModel("gemma-3-12b-it");
    }

    public async Task<string> AnalyzeProductAsync(
        Product product,
        List<IFormFile>? images = null,
        bool? expectedPassed = null,
        TextRejectReason? textRejectReason = null,
        ImageRejectReason? imageRejectReason = null)
    {
        var productJson = JsonSerializer.Serialize(product);
        string prompt;

        if (images is null)
        {
            prompt = $@"
            Você é um sistema de moderação de texto de produtos para um marketplace e produtos online.
            
            **REGRAS DE MODERAÇÃO - DEVE SER SEGUIDO À RISCA, ALTÍSSIMA PRIORIDADE:**
            
            1. **Violação de Segurança (Prioridade Máxima):**
                - Crianças. 
                - Pessoas à venda ou situações de tráfico humano. 
                - Conteúdo sexual explícito ou nudez. 
                - Violência extrema ou gore. 
                - Armas de fogo, explosivos ou itens ilegais. 
                - Drogas ou atividades ilegais. 
                - Discurso de ódio, símbolos extremistas ou conteúdo perigoso. 

            3. **Não considere como violação (Exceções):** 
                - Objetos de cozinha, ferramentas ou utensílios domésticos comuns. 
                - Brinquedos ou réplicas inofensivas de armas.

            **Formato da Resposta (DEVE SER JSON VÁLIDO):**
            {{
              ""moderation_passed"": boolean,
              ""text_rejection_reason"": ""string"",
              ""image_rejection_reason"": ""string""
            }}

            **Instruções Detalhadas para o JSON:**
            - moderation_passed: false se houver QUALQUER violação no texto. true se tudo estiver ok.
            - text_rejection_reason:
              - {TextRejectReason.PROHIBITED_CONTENT} → se o TEXTO contiver conteúdo ilegal, perigoso ou impróprio.
              - {TextRejectReason.NONE} → se não houver violação no texto.
            - image_rejection_reason:
              - {ImageRejectReason.NONE} → ***IMPORTANTE:***SEMPRE RETORNE ISTO!!!!!.

            **DADOS PARA ANÁLISE:**
            Produto: {productJson}";
        }
        else
        {
            prompt = $@"
            Você é um sistema de moderação de imagens para um marketplace e produtos online.
            
            **REGRAS DE MODERAÇÃO - DEVE SER SEGUIDO À RISCA, ALTÍSSIMA PRIORIDADE:**
            
            1. **Violação de Segurança (Prioridade Máxima):**
                - Crianças visíveis. 
                - Pessoas à venda ou situações de tráfico humano. 
                - Conteúdo sexual explícito ou nudez. 
                - Violência extrema ou gore. 
                - Armas de fogo, explosivos ou itens ilegais. 
                - Drogas ou atividades ilegais. 
                - Discurso de ódio, símbolos extremistas ou conteúdo perigoso. 
                
            2. **Não considere como violação (Exceções):** 
                - Objetos de cozinha, ferramentas ou utensílios domésticos comuns. 
                - Brinquedos ou réplicas inofensivas de armas.

            **Formato da Resposta (DEVE SER JSON VÁLIDO):**
            {{
              ""moderation_passed"": boolean,
              ""text_rejection_reason"": ""string"",
              ""image_rejection_reason"": ""string""
            }}

            **Instruções Detalhadas para o JSON:**
            - moderation_passed: false se houver QUALQUER violação (texto ou imagem). true se tudo estiver ok.
            - text_rejection_reason:
              - {TextRejectReason.PROHIBITED_CONTENT} → se o TEXTO contiver conteúdo ilegal, perigoso ou impróprio.
              - {TextRejectReason.NONE} → se não houver violação no texto.
            - image_rejection_reason:
              - {ImageRejectReason.PROHIBITED_CONTENT} → nudez, violência, armas, drogas, etc.
              - {ImageRejectReason.NO_PRODUCT_SHOWN} → Ausência de produto (ex: selfie, meme).
              - {ImageRejectReason.NONE} → se as imagens estiverem ok.

            **DADOS PARA ANÁLISE:**
            Produto: {productJson}
            Imagens: {images}";
        }

        var parts = new List<Part>
        {
            new() { Text = prompt }
        };

        foreach (var imageFile in images ?? Enumerable.Empty<IFormFile>())
        {
            try
            {
                if (imageFile == null) continue;

                if (imageFile.Length == 0) continue;

                if (string.IsNullOrEmpty(imageFile.ContentType) || !imageFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) continue;

                // Copia o conteúdo da imagem para memória
                await using var ms = new MemoryStream();
                await imageFile.CopyToAsync(ms);

                // Adiciona ao prompt como parte do conteúdo
                parts.Add(new Part
                {
                    InlineData = new Blob
                    {
                        MimeType = imageFile.ContentType,
                        Data = Convert.ToBase64String(ms.ToArray())
                    }
                });
            }
            catch (Exception ex)
            {
                // Loga o erro, mas continua o processamento das outras imagens
                Console.WriteLine($"Erro ao processar imagem '{imageFile?.FileName}': {ex.Message}");
            }
        }

        // Cria o conteúdo composto (texto + imagens)
        var content = new Content();
        content.Parts.AddRange(parts);

        // Envia tudo para o modelo
        var response = await _gemma.GenerateContentAsync(new GenerateContentRequest
        {
            Contents = { content }
        });

        var text = response.Candidates?.FirstOrDefault()?.Content?.Parts.FirstOrDefault()?.Text ?? string.Empty;

        // Limpeza do texto (remove markdown)
        var cleaned = text
            .Replace("```json", "", StringComparison.OrdinalIgnoreCase)
            .Replace("```", "")
            .Trim();

        try
        {
            using var doc = JsonDocument.Parse(cleaned);
            return doc.RootElement.GetRawText();
        }
        catch (JsonException)
        {
            return JsonSerializer.Serialize(new
            {
                moderation_passed = false,
                text_rejection_reason = $"{TextRejectReason.INVALID_RESPONSE}",
                image_rejection_reason = $"{ImageRejectReason.INVALID_RESPONSE}"
            });
        }
    }
}
