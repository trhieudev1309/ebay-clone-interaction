# EbayChat - eBay Clone Project

EbayChat is a professional e-commerce platform clone built with **ASP.NET Core MVC**. It features a robust product management system and integrated real-time communication between buyers and sellers.

##  Key Features

*   **Product Discovery**: Browse and filter products by category.
*   **Real-time Chat**: Integrated **SignalR** chat allowing buyers to initiate conversations with sellers directly from product pages.
*   **Order Management**: Full "Buy It Now" checkout flow.
*   **Resolution Center**: Dedicated dispute management system to handle transaction issues.
*   **Role-based Access**: Custom functionality for Buyers and Sellers.
*   **Persistent Sessions**: Support for distributed SQL Server caching and data protection.

##  Technical Stack

*   **Framework**: ASP.NET Core 8.0/9.0 MVC
*   **Database**: SQL Server with Entity Framework Core (Code First/Scaffold)
*   **Real-time**: SignalR Hubs
*   **Frontend**: Razor Views, Bootstrap 5, Vanilla JavaScript
*   **Caching**: Distributed SQL Server Cache & Redis (Production support)
*   **Security**: ASP.NET Data Protection & Session management

##  Project Structure

*   **/Entities**: Database models, DbContext, and Migrations.
*   **/Services**: Service interfaces defining business logic.
*   **/ServicesImpl**: Concrete implementations of business services.
*   **/Controllers**: MVC Controllers handling requests and routing.
*   **/Models**: DTOs and ViewModels for data transfer between layers.
*   **/Hubs**: SignalR Hub classes for real-time chat functionality.
*   **/wwwroot**: Static assets (CSS, JS, Images).

##  Getting Started

### 1. Database Setup
Ensure you have a SQL Server instance running. Update the connection string in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=EbayChatDb;User Id=sa;Password=your_password;TrustServerCertificate=True;"
}
```

### 2. Apply Migrations
Run the following command to set up the database schema:

```bash
dotnet ef database update
```

### 3. Run the Application
```bash
dotnet run
```
The application will be available at `https://localhost:5001` or `http://localhost:5000`.

---
*Built as part of the PRN222 course project.*
