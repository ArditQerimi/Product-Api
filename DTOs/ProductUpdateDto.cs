namespace ProductApi.DTOs;

public record ProductUpdateDto(
    string? Name = null,
    int? CategoryId = null,
    decimal? Price = null,
    int? StockQuantity = null);