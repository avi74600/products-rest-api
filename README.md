# Products REST API (.NET)

**Level 3 of 4 · Small build — A real deliverable with several pieces working together**

A production-grade RESTful API built in ASP.NET Core implementing standard CRUD operations with declarative DataAnnotation model validation, explicit RFC-compliant HTTP status codes (no 200-for-everything), and interactive Swagger / OpenAPI documentation.

---

## 🎯 Requirements Checklist

| Requirement | Implementation Details | Status |
| :--- | :--- | :---: |
| **Controller with GET, POST, PUT, DELETE** | [`ProductsController`](file:///Users/abhishekyadav/internship%20project/level3-rest-api/Controllers/ProductsController.cs) implements full CRUD with `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]` | ✅ Met |
| **Model validation on input** | `CreateProductDto` & `UpdateProductDto` enforce `[Required]`, `[StringLength]`, and `[Range]` constraints; returns standard ProblemDetails | ✅ Met |
| **Correct status codes (not 200 for everything)** | Explicitly produces `200 OK`, `201 Created` (with `Location` header), `204 No Content`, `400 Bad Request`, and `404 Not Found` | ✅ Met |
| **Exercise from Swagger or curl** | Swashbuckle Swagger UI enabled at root (`/`) and `/swagger`; verified with full `curl` test suite | ✅ Met |
| **Builds and runs cleanly** | Verified with `dotnet build` (0 warnings, 0 errors) and `dotnet run` | ✅ Met |

---

## 🌐 HTTP Endpoints & Status Codes

| Method | Endpoint | Description | Success Code | Error Codes |
| :--- | :--- | :--- | :---: | :---: |
| `GET` | `/api/products` | Retrieve all products (supports `category`, `minPrice`, `maxPrice` query filters) | **`200 OK`** | `400 Bad Request` |
| `GET` | `/api/products/{id}` | Retrieve a single product by its integer ID | **`200 OK`** | `400 Bad Request`, `404 Not Found` |
| `POST` | `/api/products` | Create a new product with model validation | **`201 Created`** *(+ Location header)* | `400 Bad Request` |
| `PUT` | `/api/products/{id}` | Update an existing product by ID | **`200 OK`** | `400 Bad Request`, `404 Not Found` |
| `DELETE` | `/api/products/{id}` | Delete a product from the catalog | **`204 No Content`** | `400 Bad Request`, `404 Not Found` |

---

## 🛡️ Model Validation Rules

Applied on both `CreateProductDto` and `UpdateProductDto`:

```csharp
[Required(ErrorMessage = "Product name is required.")]
[StringLength(100, MinimumLength = 2, ErrorMessage = "Product name must be between 2 and 100 characters.")]
public string Name { get; set; }

[StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
public string Description { get; set; }

[Required(ErrorMessage = "Category is required.")]
[StringLength(50, MinimumLength = 2, ErrorMessage = "Category must be between 2 and 50 characters.")]
public string Category { get; set; }

[Required(ErrorMessage = "Price is required.")]
[Range(0.01, 100000.00, ErrorMessage = "Price must be strictly positive and cannot exceed $100,000.00.")]
public decimal Price { get; set; }

[Range(0, 100000, ErrorMessage = "Stock quantity cannot be negative.")]
public int StockQuantity { get; set; }
```

When validation fails, ASP.NET Core automatically returns **`400 Bad Request`** with an RFC 9110 validation problem payload indicating the exact invalid properties.

---

## 🚀 How to Run

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (version 8.0, 9.0, or 10.0)

### 1. Clone & Run
```bash
# Clone the repository
git clone https://github.com/avi74600/products-rest-api.git
cd products-rest-api

# Build the project
dotnet build

# Launch the API
dotnet run --launch-profile http
```

### 2. Interactive Swagger UI
Open your browser to:
👉 **`http://localhost:5055/`** (or `http://localhost:5055/swagger`)

---

## 🧪 Exercising the API with `curl`

### 1. GET All Products (`200 OK`)
```bash
curl -i http://localhost:5055/api/products
```

### 2. GET by ID (`200 OK`)
```bash
curl -i http://localhost:5055/api/products/1
```

### 3. GET Non-Existent ID (`404 Not Found`)
```bash
curl -i http://localhost:5055/api/products/999
```

### 4. POST Create Product (`201 Created`)
```bash
curl -i -X POST http://localhost:5055/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Smart Fitness Watch",
    "description": "Continuous heart-rate monitor with GPS tracking.",
    "category": "Fitness",
    "price": 149.99,
    "stockQuantity": 30
  }'
```
*Note: Response contains `Location: http://localhost:5055/api/Products/5` header.*

### 5. POST with Invalid Model (`400 Bad Request`)
```bash
curl -i -X POST http://localhost:5055/api/products \
  -H "Content-Type: application/json" \
  -d '{
    "name": "",
    "category": "",
    "price": -15.00,
    "stockQuantity": -5
  }'
```

### 6. PUT Update Product (`200 OK`)
```bash
curl -i -X PUT http://localhost:5055/api/products/1 \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Noise-Canceling Wireless Headphones (Gen 2)",
    "description": "Upgraded battery life and improved ANC microphones.",
    "category": "Electronics",
    "price": 219.99,
    "stockQuantity": 35
  }'
```

### 7. DELETE Product (`204 No Content`)
```bash
curl -i -X DELETE http://localhost:5055/api/products/3
```

### 8. DELETE Non-Existent Product (`404 Not Found`)
```bash
curl -i -X DELETE http://localhost:5055/api/products/999
```
