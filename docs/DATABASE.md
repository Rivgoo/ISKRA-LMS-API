# Iskra Database Architecture and Workflow

This document outlines the database architecture for the Iskra platform. The design is modular, allowing for easy extension and support for multiple database providers (e.g., MariaDB, PostgreSQL, SQL Server).

## Core Principles

-   **Provider Agnostic:** The core application logic does not depend on a specific database.
-   **Modularity:** Each database provider is implemented as a separate, self-contained module.
-   **Clean Architecture:** Database logic is strictly confined to the Infrastructure layer.
-   **Configuration over Code:** Schema details like table names can be overridden per provider without changing shared code.

## Component Breakdown

The database layer is split into two main parts: a shared infrastructure project and specific provider implementations.

### 1. `Iskra.Infrastructure.Shared`

This project is the foundation for all database modules. It is provider-agnostic.

-   **`AppDbContextBase.cs`**: An abstract `DbContext` that contains:
    -   `DbSet` properties for core entities (`User`, `Role`, etc.).
    -   Automatic timestamping logic (`IAuditable`) in `SaveChangesAsync`.
    -   Logic to apply **default** entity configurations from its own assembly.
-   **`/Configurations`**: Contains default `IEntityTypeConfiguration<T>` implementations for core entities. These define standard table names (e.g., `Users`), relationships, and constraints.

### 2. `Iskra.Infrastructure.MariaDb` (Example Provider)

This is a concrete implementation module for a specific database.

-   **`MariaDbContext.cs`**: Inherits from `AppDbContextBase` and is specific to MariaDB. Its primary role is to apply its own configurations **after** the base configurations, allowing for overrides.
-   **`MariaDbModule.cs`**: The `IModule` implementation that:
    -   Reads the connection string from its own `json` config file.
    -   Registers the `MariaDbContext` with the correct provider (`UseMySql`).
    -   Handles automatic database migrations on startup if enabled.
-   **`MariaDbContextFactory.cs`**: An `IDesignTimeDbContextFactory` implementation. This is crucial for allowing `dotnet-ef` tools to create migrations without needing to run the main Host application.
-   **`/Configurations`**: (Optional) Contains provider-specific overrides for entity configurations.

## Workflow Guide

### Working with Migrations

Migrations are managed within each specific database module (e.g., `Iskra.Infrastructure.MariaDb`).

**1. Install the EF Tool Locally:**
This adds the `dotnet-ef` tool to the manifest.
```bash
dotnet tool install dotnet-ef
```

**2. Restore Local Tools (for new clones or CI/CD):**
If you pull the project on a new machine, you must restore the local tools.
```bash
dotnet tool restore
```

#### Creating a New Migration

To create a new migration, run the `dotnet ef migrations add` command from the **root of the solution**.

```bash
dotnet ef migrations add <MigrationName> --project src/Infrastructure/Iskra.Infrastructure.MariaDb
```

**Example:**
```bash
dotnet ef migrations add AddUserPhoneNumber --project src/Infrastructure/Iskra.Infrastructure.MariaDb
```

> **Note:** This works because the `MariaDbContextFactory` inside the project tells the EF tools how to create the `DbContext` and read the connection string from the local JSON configuration.

#### Applying Migrations

There are two ways to apply migrations:

1.  **Automatic (Recommended for Development)**
    Set `"AutoMigrate": true` in the module's JSON configuration (`Iskra.Infrastructure.MariaDb.json`). The application will apply any pending migrations on startup.

2.  **Manual (Recommended for Production)**
    Set `"AutoMigrate": false` and run the command from the solution root:
    ```bash
    dotnet ef database update --project src/Infrastructure/Iskra.Infrastructure.MariaDb
    ```

### Overriding Table Configurations

Our architecture uses a "last one wins" principle for configuration.

1.  **Default Configuration (`Shared`):**
    ```csharp
    // src/Infrastructure/Iskra.Infrastructure.Shared/Configurations/UserConfiguration.cs
    builder.ToTable("Users"); 
    ```

2.  **Override Configuration (`MariaDb`):**
    ```csharp
    // src/Infrastructure/Iskra.Infrastructure.MariaDb/Configurations/MariaDbUserConfiguration.cs
    builder.ToTable("users"); // Override to lowercase
    ```

3.  **How it works in `MariaDbContext`:**
    ```csharp
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. Applies default configs ("Users") from Shared assembly
        base.OnModelCreating(modelBuilder); 

        // 2. Applies specific configs ("users") from this assembly, overriding the default
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); 
    }
    ```

### Adding a New Database Provider (e.g., PostgreSQL)

1.  Create a new project `Iskra.Infrastructure.PostgreSql`.
2.  Add NuGet package `Npgsql.EntityFrameworkCore.PostgreSQL`.
3.  Create `PostgreSqlDbContext` inheriting from `AppDbContextBase`.
4.  Implement `PostgreSqlModule` to register the context with `UseNpgsql`.
5.  Implement `PostgreSqlDbContextFactory` for migrations.
6.  (Optional) Add override configurations in a `/Configurations` folder.
7.  Add `"Iskra.Infrastructure.PostgreSql"` to `EnabledModules` in the Host's `appsettings.json`.