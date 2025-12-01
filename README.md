# Product API – .NET 9 Web API with JWT, EF Core In-Memory & Scalar UI

## Features

- .NET 9 + ASP.NET Core Web API
- JWT Authentication
- Full CRUD for Products & Categories
- Advanced filtering, paging, sorting
- `InStock` field only shown when StockQuantity > 0
- Global exception handling
- Beautiful interactive docs with **Scalar** 
- In-memory database with seeded data

Live API Documentation (Recommended Testing Method)

After starting the app, open:

https://localhost:7002/scalar/v1

or

http://localhost:5084/scalar/v1

##  Login Credentials (for JWT Authentication)

Use these credentials to log in through Scalar or any API client:

{
  "username": "admin",
  "password": "adminpw"
}


After logging in, copy the generated JWT token and authorize your requests.

## How to Run

```bash
git clone https://github.com/ArditQerimi/product-api.git
cd Product-Api

dotnet run

