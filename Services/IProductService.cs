namespace ProductsApi.Services;

using ProductsApi.Models;

/// <summary>
/// Service contract for managing product catalog operations.
/// </summary>
public interface IProductService
{
    IEnumerable<Product> GetAll(string? category = null, decimal? minPrice = null, decimal? maxPrice = null);
    Product? GetById(int id);
    Product Create(CreateProductDto dto);
    Product? Update(int id, UpdateProductDto dto);
    bool Delete(int id);
}
