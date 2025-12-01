using ProductApi.DTOs;

namespace ProductApi.Services;

public interface IProductService
{
    Task<IEnumerable<object>> GetAllAsync(
        int? categoryId = null,        
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStock = null,
        string? name = null,
        string? categoryName = null,
        int page = 1,
        int pageSize = 10,
        string? sortBy = null,          
        string sortOrder = "asc");

    Task<object?> GetByIdAsync(int id);
    Task<object> CreateAsync(ProductCreateDto dto);
    Task<bool> UpdateAsync(int id, ProductUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}