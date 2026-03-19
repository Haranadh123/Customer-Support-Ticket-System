# Customer Support Ticket System

A desktop application for managing support tickets, built with C# (WPF), ASP.NET Web API, and Microsoft SQL Server.

## Technology Stack
- **Backend**: ASP.NET Web API / Minimal API (.NET 8.0)
- **Frontend**: C# WPF (Desktop Application)
- **Database**: Microsoft SQL Server (LocalDB)
- **ORM**: Entity Framework Core
- **Communication**: JSON over HTTP (REST)

## Project Structure
- `CustomerSupport.API`: Handles business logic, authentication, and database interaction.
- `CustomerSupport.Client`: Provides the UI for Users and Admins.
- `CustomerSupport.Shared`: Common data models and DTOs shared between the API and Client.

## Setup Instructions

### 1. Database Setup
1. Ensure **Microsoft SQL Server** (or SQL Server LocalDB) is installed and running.
2. Execute the script located at `scripts/schema.sql` in **SQL Server Management Studio (SSMS)** or VS Code to create the database and seed the initial users.
3. The API is configured to use `(localdb)\mssqllocaldb` by default. Update the connection string in `CustomerSupport.API/appsettings.json` if your SQL Server instance differs.

### 2. Running the API
1. Open a terminal in the `CustomerSupport.API` directory.
2. Run `dotnet run`.
3. The API will typically run on `http://localhost:5119`.

### 3. Running the Desktop Application
1. Open a terminal in the `CustomerSupport.Client` directory.
2. Ensure the API is running.
3. Run `dotnet run`.

## Default Credentials
- **Admin**: `admin1` / `password123`
- **User**: `user1` / `password123`

## Business Rules
- Ticket numbers are auto-generated as `TKT-YYYYMMDD-XXXX`.
- Status flow: Open -> In Progress -> Closed.
- Users can only view their own tickets.
- Admins can view all tickets and assign them to other admins.
- Internal comments are visible only to Admins.
- All status changes are logged in the history table.
