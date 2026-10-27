namespace ProductsApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using ProductsApi.Models;
using ProductsApi.Services;

/// <summary>
/// Controller providing standard RESTful CRUD operations (GET, POST, PUT, DELETE)
/// with comprehensive model validation and explicit HTTP status codes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all products with optional query filtering.
    /// </summary>
    /// <response code="200">Returns the list of products matching criteria.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Product>> GetAll(
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice)
    {
        if (minPrice.HasValue && minPrice.Value < 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Query Parameter",
                Detail = "minPrice cannot be negative.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (maxPrice.HasValue && minPrice.HasValue && maxPrice.Value < minPrice.Value)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Query Parameter",
                Detail = "maxPrice cannot be less than minPrice.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var products = _productService.GetAll(category, minPrice, maxPrice);
        return Ok(products);
    }

    /// <summary>
    /// Retrieves a single product by its unique identifier.
    /// </summary>
    /// <param name="id">Product integer ID.</param>
    /// <response code="200">Product found and returned.</response>
    /// <response code="400">ID is non-positive.</response>
    /// <response code="404">Product with given ID does not exist.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Product> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Identifier",
                Detail = "Product ID must be a positive integer greater than zero.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var product = _productService.GetById(id);
        if (product == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource Not Found",
                Detail = $"Product with ID {id} was not found.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(product);
    }

    /// <summary>
    /// Creates a new product catalog item with validated input.
    /// </summary>
    /// <param name="dto">Product creation payload.</param>
    /// <response code="201">Product successfully created with Location header.</response>
    /// <response code="400">Model validation failed on payload fields.</response>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<Product> Create([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var created = _productService.Create(dto);
        _logger.LogInformation("Created new product ID {Id}: {Name}", created.Id, created.Name);

        // Returns HTTP 201 Created with Location: /api/products/{id} header
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Updates an existing product catalog item.
    /// </summary>
    /// <param name="id">Product integer ID to update.</param>
    /// <param name="dto">Updated product fields.</param>
    /// <response code="200">Product successfully updated.</response>
    /// <response code="400">Invalid ID or payload model validation failure.</response>
    /// <response code="404">Product with given ID was not found.</response>
    [HttpPut("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public ActionResult<Product> Update(int id, [FromBody] UpdateProductDto dto)
    {
        if (id <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Identifier",
                Detail = "Product ID must be greater than zero.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var updated = _productService.Update(id, dto);
        if (updated == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource Not Found",
                Detail = $"Cannot update. Product with ID {id} does not exist.",
                Status = StatusCodes.Status404NotFound
            });
        }

        _logger.LogInformation("Updated product ID {Id}: {Name}", updated.Id, updated.Name);
        return Ok(updated);
    }

    /// <summary>
    /// Deletes a product from the catalog.
    /// </summary>
    /// <param name="id">Product integer ID to delete.</param>
    /// <response code="204">Product successfully deleted (No Content).</response>
    /// <response code="400">ID is non-positive.</response>
    /// <response code="404">Product with given ID was not found.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid Identifier",
                Detail = "Product ID must be greater than zero.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        bool deleted = _productService.Delete(id);
        if (!deleted)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Resource Not Found",
                Detail = $"Cannot delete. Product with ID {id} does not exist.",
                Status = StatusCodes.Status404NotFound
            });
        }

        _logger.LogInformation("Deleted product ID {Id}", id);
        // Standard REST response for DELETE: 204 No Content
        return NoContent();
    }
}
