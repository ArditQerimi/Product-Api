namespace ProductApi.DTOs;

public record ProductReadNoStockDto(
    int Id,
    string Name,
    int CategoryId,
    string CategoryName,
    decimal Price,
    DateTime CreatedAt
);