namespace ProductsApi.Services;

using System.Collections.Concurrent;
using ProductsApi.Models;

/// <summary>
/// In-memory thread-safe implementation of <see cref="IProductService"/>.
/// Pre-seeded with realistic initial catalog products.
/// </summary>
public class InMemoryProductService : IProductService
{
    private readonly ConcurrentDictionary<int, Product> _products = new();
    private int _nextId = 0;

    public InMemoryProductService()
    {
        SeedInitialData();
    }

    private void SeedInitialData()
    {
        Create(new CreateProductDto
        {
            Name = "Noise-Canceling Wireless Headphones",
            Description = "Over-ear wireless headphones with active noise cancellation and 30-hour battery life.",
            Category = "Electronics",
            Price = 199.99m,
            StockQuantity = 45
        });

        Create(new CreateProductDto
        {
            Name = "Mechanical Gaming Keyboard",
            Description = "RGB backlit mechanical keyboard with tactile blue switches and detachable USB-C cable.",
            Category = "Electronics",
            Price = 89.50m,
            StockQuantity = 20
        });

        Create(new CreateProductDto
        {
            Name = "Stainless Steel French Press",
            Description = "Double-wall insulated 34oz coffee maker made of food-grade 304 stainless steel.",
            Category = "Home & Kitchen",
            Price = 29.99m,
            StockQuantity = 50
        });

        Create(new CreateProductDto
        {
            Name = "Ergonomic Mesh Office Chair",
            Description = "Breathable mesh task chair with adjustable lumbar support and flip-up armrests.",
            Category = "Office Furniture",
            Price = 189.00m,
            StockQuantity = 15
        });
    }

    public IEnumerable<Product> GetAll(string? category = null, decimal? minPrice = null, decimal? maxPrice = null)
    {
        var query = _products.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category.Equals(category.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        return query.OrderBy(p => p.Id);
    }

    public Product? GetById(int id)
    {
        _products.TryGetValue(id, out var product);
        return product;
    }

    public Product Create(CreateProductDto dto)
    {
        int id = Interlocked.Increment(ref _nextId);
        var product = new Product
        {
            Id = id,
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty,
            Category = dto.Category.Trim(),
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null
        };

        _products[id] = product;
        return product;
    }

    public Product? Update(int id, UpdateProductDto dto)
    {
        if (!_products.TryGetValue(id, out var existing))
        {
            return null;
        }

        var updated = new Product
        {
            Id = id,
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty,
            Category = dto.Category.Trim(),
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            CreatedAt = existing.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        _products[id] = updated;
        return updated;
    }

    public bool Delete(int id)
    {
        return _products.TryRemove(id, out _);
    }
}
