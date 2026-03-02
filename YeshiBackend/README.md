# Yeshi Platform (ASP.NET Core + PostgreSQL)

## Hosting compatibility
This solution is built for:
- Windows web hosting
- Plesk Control Panel
- Shared hosting constraints
- PostgreSQL (including Supabase)

No Node.js, MongoDB, or Firebase is required.

## Architecture (3-tier)
- Frontend: HTML/CSS/JavaScript in `design-to-finish/frontend`
- Backend: ASP.NET Core MVC-style controllers + REST APIs in `YeshiBackend`
- Database: PostgreSQL via Entity Framework Core (`Npgsql`)

Business logic is in service classes under `Services/` and separated from API controllers.

## Implemented database tables
- `Users`
- `Admins`
- `Categories`
- `Products`
- `Orders`
- `OrderItems`

## Implemented relationships
- Category → many Products
- User → many Orders
- Order → many OrderItems
- OrderItem → one Product
- Admin → one User

Each table includes timestamp fields (`CreatedAt`, `UpdatedAt`) and orders use status values:
- `Pending`, `Processing`, `Shipped`, `Completed`

## Security features
- Secure registration/login
- Password hashing via BCrypt
- JWT authentication (`x-auth-token` supported)
- Role-based authorization (`Admin`, `Customer`)
- Input validation through DTOs + annotations
- Centralized exception handling middleware

## API modules
- Auth:
  - `POST /api/auth/register`
  - `POST /api/auth/login`
- Public catalog:
  - `GET /api/catalog/categories`
  - `GET /api/catalog/products`
  - `GET /api/catalog/products/{id}`
- Session cart + checkout:
  - `GET /api/cart`
  - `POST /api/cart/items`
  - `PUT /api/cart/items/{productId}`
  - `DELETE /api/cart/items/{productId}`
  - `POST /api/cart/checkout`
- Orders:
  - `GET /api/orders`
- Admin:
  - `GET/POST /api/admin/categories`
  - `GET/POST/PUT/DELETE /api/admin/products`
  - `GET /api/admin/orders`
  - `PUT /api/admin/orders/{id}/status`

## Frontend pages created
- Public: `shop/home.html`, `shop/products.html`, `shop/product-details.html`, `shop/cart.html`, `shop/checkout.html`, login/register pages
- Admin: `admin-panel/dashboard.html`, `admin-panel/add-product.html`, `admin-panel/edit-product.html`, `admin-panel/orders.html`

## PostgreSQL configuration
Edit:
- `appsettings.json`
- `appsettings.Development.json`

Example:
```json
"DefaultConnection": "Host=db.bsmpncihhfczbvsmvdbf.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require;Trust Server Certificate=true;"
```

For cloud deployment, prefer environment variables instead of storing secrets in files:
- `ConnectionStrings__DefaultConnection`
- `Jwt__Key`
- `FrontendOrigins__0` (e.g. your Vercel domain)
- `FrontendOrigins__1` (optional additional domain)

## Run locally
```bash
dotnet restore
dotnet run
```

## Plesk/shared hosting notes
- Publish as framework-dependent deployment.
- Configure `ASPNETCORE_ENVIRONMENT=Production`.
- Set production PostgreSQL connection string in `appsettings.Production.json` or Plesk environment variables.
- Set a strong production `Jwt:Key`.
- Ensure write permission for `Uploads/` folder.
