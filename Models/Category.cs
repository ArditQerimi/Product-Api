namespace ProductApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<Product> Products { get; set; } = new();

    }
}
