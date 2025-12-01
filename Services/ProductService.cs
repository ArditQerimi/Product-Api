using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.DTOs;
using ProductApi.Models;

namespace ProductApi.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<object>> GetAllAsync(
    int? categoryId = null,
    decimal? minPrice = null,
    decimal? maxPrice = null,
    bool? inStock = null,
    string? name = null,          
    string? categoryName = null, 
    int page = 1,
    int pageSize = 10,
    string? sortBy = null,
    string sortOrder = "asc")
    {
        var query = _db.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (inStock.HasValue)
            query = query.Where(p => inStock.Value ? p.StockQuantity > 0 : p.StockQuantity == 0);

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(p => p.Name.Contains(name.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(categoryName))
            query = query.Where(p => p.Category != null &&
                                    p.Category.Name.Contains(categoryName.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            query = (sortBy.ToLower(), sortOrder?.ToLower()) switch
            {
                ("price", "desc") => query.OrderByDescending(p => p.Price),
                ("price", _) => query.OrderBy(p => p.Price),
                ("name", "desc") => query.OrderByDescending(p => p.Name),
                ("name", _) => query.OrderBy(p => p.Name),
                ("category", "desc") => query.OrderByDescending(p => p.Category!.Name),
                ("category", _) => query.OrderBy(p => p.Category!.Name),
                ("stock", "desc") => query.OrderByDescending(p => p.StockQuantity),
                ("stock", _) => query.OrderBy(p => p.StockQuantity),
                _ => query.OrderBy(p => p.Id)
            };
        }
        else
        {
            query = query.OrderBy(p => p.Id);
        }

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => p.StockQuantity > 0
                ? new ProductReadDto(
                    p.Id,
                    p.Name,
                    p.CategoryId,
                    p.Category!.Name,
                    p.Price,
                    p.StockQuantity,
                    true,
                    p.CreatedAt
                  ) as object
                : new ProductReadNoStockDto(
                    p.Id,
                    p.Name,
                    p.CategoryId,
                    p.Category!.Name,
                    p.Price,
                    p.CreatedAt
                  ) as object)
            .ToListAsync();

        return result;
    }

    public async Task<object?> GetByIdAsync(int id)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) throw new Exception("Product not found");

        return product.StockQuantity > 0 ? new ProductReadDto(
            product.Id,
            product.Name,
            product.CategoryId,
            product.Category.Name,
            product.Price,
            product.StockQuantity,
            product.StockQuantity > 0,
            product.CreatedAt
        ) :
        new ProductReadNoStockDto(
                product.Id,
                product.Name,
                product.CategoryId,
                product.Category.Name,
                product.Price,
                product.CreatedAt
        ) as object;
    }

    public async Task<object> CreateAsync(ProductCreateDto dto)
    {
        var categoryExists = await _db.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists)
            throw new KeyNotFoundException($"Category with id {dto.CategoryId} not found.");

        var product = new Product
        {
            Name = dto.Name,
            CategoryId = dto.CategoryId,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            CreatedAt = DateTime.UtcNow
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        await _db.Entry(product).Reference(p => p.Category).LoadAsync();

        return product.StockQuantity > 0 ? new ProductReadDto(
            product.Id,
            product.Name,
            product.CategoryId,
            product.Category.Name,
            product.Price,
            product.StockQuantity,
            product.StockQuantity > 0,
            product.CreatedAt
        ) 
        : new ProductReadNoStockDto(
                product.Id,
                product.Name,
                product.CategoryId,
                product.Category.Name,
                product.Price,
                product.CreatedAt
        ) as object;
    }

    public async Task<bool> UpdateAsync(int id, ProductUpdateDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null) throw new Exception("Product not found");

        if (dto.Name != null) product.Name = dto.Name;
        if (dto.Price.HasValue) product.Price = dto.Price.Value;
        if (dto.StockQuantity.HasValue) product.StockQuantity = dto.StockQuantity.Value;
        if (dto.CategoryId.HasValue)
        {
            var categoryExists = await _db.Categories.AnyAsync(c => c.Id == dto.CategoryId.Value);
            if (!categoryExists)
                throw new KeyNotFoundException($"Category with id {dto.CategoryId.Value} not found.");

            product.CategoryId = dto.CategoryId.Value;
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product == null)
            throw new Exception("Product not found");

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return true;
    }
}