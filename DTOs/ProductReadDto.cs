namespace ProductApi.DTOs;

public record ProductReadDto(
    int Id,
    string Name,
    int CategoryId,
    string Category,
    decimal Price,
    int StockQuantity,
    bool? InStock,
    DateTime CreatedAt);