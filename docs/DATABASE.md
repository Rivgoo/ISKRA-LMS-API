# Iskra Database Architecture

This document outlines the database architecture for the Iskra Modular Platform. The design allows the platform to be database-agnostic while ensuring type safety, modularity, and a unified development workflow.

## Core Principles

1.  **Unified Domain:** All Entities (e.g., `User`, `Role`, `Course`) are defined in specific modules but are loaded into a **Single Context** at runtime.
2.  **Modular Provider:** The database engine (MariaDB, PostgreSQL) is a pluggable module. It applies global conventions (e.g., snake_case) without modifying core entities.
3.  **Centralized Migrations:** Migrations are generated based on the **Build Output**. The infrastructure layer scans the compiled module DLLs in the build directory to discover entities dynamically.
4.  **Scoped Configuration:** Database connection strings and settings are managed in the Host's `appsettings.json`, scoped by the module name.

---

## Architecture Overview

### 1. Entity Definition (Domain Layer)
Entities are defined in their respective modules. They are strictly POCO classes.

*   **Shared Entities:** Located in `src/Core/Iskra.Core.Domain` (e.g., `User`, `Role`).
*   **Module Entities:** Located in `src/Modules/Iskra.Modules.[Feature]/Domain` (e.g., `Course`, `Lesson`).

### 2. Entity Configuration (Persistence Layer)
We use `IEntityTypeConfiguration<T>` to define constraints.

*   **Shared Configurations:** `src/Infrastructure/Iskra.Infrastructure.Shared` contains configs for Core entities.
*   **Module Configurations:** Feature modules contain their own configuration classes implementing `IModelConfiguration`.

### 3. Database Provider (Infrastructure Layer)
The concrete database logic resides in `src/Modules/Iskra.Modules.[Provider]`.

*   **Example:** `Iskra.Modules.MariaDb`
*   **Responsibilities:**
    *   Registers the `DbContext` with the DI container using settings from `appsettings.json`.
    *   Applies global naming conventions (e.g., converting `PascalCase` properties to `snake_case` columns).
    *   Executes migrations on startup (optional).

---

## Configuration

Database settings are stored in the Host's central `appsettings.json`. Each provider module reads from its specific section.

**Example `appsettings.json`:**
```json
{
  "Iskra.Modules.MariaDb": {
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=iskra_lms;User=root;Password=root;"
    },
    "DatabaseSettings": {
      "AutoMigrate": true
    }
  }
}
```

This structure allows for overriding settings via Environment Variables in Docker/Cloud environments (e.g., `Iskra__Modules__MariaDb__ConnectionStrings__DefaultConnection`).

---

## Migration Workflow

Because Iskra loads modules dynamically, EF Core cannot automatically find entities at design time. We use a custom **Design-Time Factory** that loads compiled module DLLs and configuration from the `build` folder.

### Prerequisites

1.  **Build the Solution First:**
    The migration tool looks for DLLs and `appsettings.json` in `root/build`. You **must** build the project before creating a migration.
    ```bash
    dotnet build
    ```

2.  **Install EF Tool:**
    ```bash
    dotnet tool restore
    ```

### Creating a New Migration

To create a migration, target the **Concrete Infrastructure Module** (e.g., MariaDb).

```bash
dotnet ef migrations add <MigrationName> --project src/Modules/Iskra.Modules.MariaDb
```

**What happens under the hood:**
1.  `MariaDbContextFactory` starts.
2.  It locates the `build` directory.
3.  It loads the centralized `appsettings.json` and finds the connection string under `Iskra.Modules.MariaDb`.
4.  It scans and loads all `Iskra.Modules.*.dll` files into memory.
5.  It constructs a unified `MariaDbContext` containing every entity in the system.
6.  It compares this state with the previous snapshot and generates the migration class.

### Applying Migrations

**Option A: Automatic (Development)**
Enable `AutoMigrate` in `appsettings.json` under the module section. The provider module will migrate the database when the application starts.

**Option B: Manual (Production)**
Run the update command:
```bash
dotnet ef database update --project src/Modules/Iskra.Modules.MariaDb
```

---

## Workflow: Adding a New Entity

1.  **Define Class:** Create the entity class in `Domain/Entities`.
2.  **Define Config:** Create the `IEntityTypeConfiguration` in `Infrastructure/Persistence`.
    *   *Note:* If inside a module, ensure you create a class implementing `IModelConfiguration` to register the configuration.
3.  **Build:** Run `dotnet build`.
4.  **Add Migration:** Run the `dotnet ef migrations add` command shown above.

## Workflow: Adding a New Database Provider

To add support for a new database (e.g., PostgreSQL):

1.  Create `src/Modules/Iskra.Modules.PostgreSql`.
2.  Inherit `AppDbContextBase` -> `PostgreSqlDbContext`.
3.  Implement `BaseDesignTimeFactory<PostgreSqlDbContext>` specifying the config section name (e.g., `Iskra.Modules.PostgreSql`).
4.  Implement `IModule` to register `AddDbContext<AppDbContextBase, PostgreSqlDbContext>`.
5.  Add the new module to `EnabledModules` in `appsettings.json`.

---

## Troubleshooting

**"Connection string missing" during migration:**
*   Ensure `appsettings.json` exists in `root/build`.
*   Ensure the section name in `appsettings.json` matches the `ModuleConfigSectionName` defined in your Factory.

**"No entities found" during migration:**
*   Did you run `dotnet build`?
*   Check `src/Core/Iskra.Core.Contracts/Constants/PathConstants.cs` to ensure the build path matches your system.
*   Ensure your module project file has the correct output path configuration (handled by `Directory.Build.props`).