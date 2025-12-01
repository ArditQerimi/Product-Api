namespace ProductApi.DTOs;

public record ProductCreateDto(
    string Name,
    int CategoryId,
    decimal Price,
    int StockQuantity = 0);