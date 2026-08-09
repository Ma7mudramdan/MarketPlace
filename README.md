# Marketplace - Online Marketplace Application

## Project Overview
A full-featured online marketplace where users can buy and sell products.

## Project Structure
- **Domain**: Core business entities and domain logic
- **Application**: Business services, DTOs, and interfaces
- **Infrastructure**: Data access, repositories, and external services
- **Web**: MVC Controllers, Views, and frontend assets

## Architecture
- Clean Architecture
- Repository Pattern
- Service Layer Pattern
- Dependency Injection

## Technology Stack
- ASP.NET Core MVC 8.0
- Entity Framework Core 8.0
- SQL Server
- ASP.NET Core Identity
- Serilog (Logging)
- AutoMapper
- Bootstrap 5

## Features (Planned)
- User Authentication & Authorization
- Product Management (CRUD)
- Category Management
- Product Search & Filtering
- Shopping Cart
- Order Management
- Product Reviews & Ratings
- Admin Dashboard

## Progress
- ✅ Project Setup (Week 1, Day 1)
- ⬜ Domain Entities
- ⬜ Database Context
- ⬜ Repository Pattern
- ⬜ Service Layer
- ⬜ MVC Controllers
- ⬜ Views
- ⬜ Advanced Features

## Getting Started
1. Clone the repository
2. Update connection string in appsettings.json
3. Run migrations: `dotnet ef database update`
4. Run the application: `dotnet run --project Marketplace.Web`

