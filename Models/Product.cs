
using System.Text.Json.Serialization;

namespace ProductApi.Models;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public int CategoryId { get; set; }

    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedAt { get; set; }


    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? InStock => StockQuantity > 0 ? true : null;

    public Category Category { get; set; } = null!;
}