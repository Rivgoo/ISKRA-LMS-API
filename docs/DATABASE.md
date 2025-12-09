# Iskra Database Architecture

This document outlines the database architecture for the Iskra Modular Platform. The design allows the platform to be database-agnostic while ensuring type safety, modularity, and a unified development workflow.

## Core Principles

1.  **Unified Domain:** All Entities (`User`, `Role`, `Course`) are defined in specific modules but are loaded into a **Single Context** at runtime.
2.  **Modular Provider:** The database engine (MariaDB, PostgreSQL) is a pluggable module. It can apply global conventions (e.g., snake_case) without modifying core entities.
3.  **Centralized Migrations:** Migrations are generated based on the **Build Output**. The infrastructure layer scans the compiled module DLLs to discover entities dynamically.
4.  **Decoupled Configuration:** Entities define their own constraints (`MaxLength`, `Required`), but the Provider defines physical storage rules (`Table Names`, `Column Types`).

---

## Architecture Overview

### 1. Entity Definition (Domain Layer)
Entities are defined in their respective modules (Core or Feature Modules). They are strictly POCO classes.

*   **Shared Entities:** Located in `src/Core/Iskra.Core.Domain` (e.g., `User`, `Role`).
*   **Module Entities:** Located in `src/Modules/Iskra.Modules.[Feature]/Domain` (e.g., `Course`, `Lesson`).

### 2. Entity Configuration (Persistence Layer)
We use `IEntityTypeConfiguration<T>` to define constraints.
*   **Shared Configurations:** `src/Infrastructure/Iskra.Infrastructure.Shared` contains configs for Core entities.
*   **Module Configurations:** Feature modules contain their own configuration classes implementing `IModelConfiguration`.

### 3. Database Provider (Infrastructure Layer)
The concrete database logic resides in `src/Modules/Iskra.Modules.Infrastructure.[Provider]`.
*   **Example:** `Iskra.Modules.MariaDb`
*   **Responsibilities:**
    *   Registers the `DbContext` with the DI container.
    *   Applies global naming conventions (e.g., converting `PascalCase` properties to `snake_case` columns).
    *   Executes migrations on startup (optional).

---

## Migration Workflow

Because Iskra loads modules dynamically, EF Core cannot automatically find your entities at design time. We use a custom **Design-Time Factory** that loads compiled module DLLs from the `build` folder.

### Prerequisites
1.  **Build the Solution First:**
    The migration tool looks for DLLs in `root/build/Modules`. You **must** build the project before creating a migration.
    ```bash
    dotnet build
    ```

2.  **Install EF Tool:**
    ```bash
    dotnet tool restore
    ```

### Creating a Migration

To create a migration, target the **Concrete Infrastructure Module** (e.g., MariaDb).

```bash
dotnet ef migrations add <MigrationName> --project src/Modules/Iskra.Modules.MariaDb
```

**What happens under the hood:**
1.  `MariaDbContextFactory` starts.
2.  It locates the `build/Modules` directory.
3.  It scans and loads all `Iskra.Modules.*.dll` files into memory.
4.  It discovers all `IModelConfiguration` implementations (from Users, Auth, Validation, etc.).
5.  It constructs a unified `MariaDbContext` containing every entity in the system.
6.  It compares this state with the previous snapshot and generates the migration class.

### Applying Migrations

**Option A: Automatic (Development)**
Enable `AutoMigrate` in `Iskra.Modules.MariaDb.json`. The module will migrate the DB when the Host starts.

**Option B: Manual (Production)**
Run the update command:
```bash
dotnet ef database update --project src/Modules/Iskra.Modules.MariaDb
```

---

## Adding a New Entity

1.  **Define Class:** Create the entity class in `Domain/Entities`.
2.  **Define Config:** Create the `IEntityTypeConfiguration` in `Infrastructure/Persistence`.
    *   *Note:* If inside a module, create a class implementing `IModelConfiguration` to register it.
3.  **Build:** Run `dotnet build`.
4.  **Add Migration:** Run the `dotnet ef migrations add` command shown above.

## Adding a New Database Provider

To add support for a new database (e.g., PostgreSQL):

1.  Create `src/Modules/Iskra.Modules.Infrastructure.PostgreSql`.
2.  Inherit `AppDbContextBase` -> `PostgreSqlDbContext`.
3.  Implement `BaseDesignTimeFactory<PostgreSqlDbContext>`.
4.  Implement `IModule` to register `AddDbContext<AppDbContextBase, PostgreSqlDbContext>`.
5.  Add to `EnabledModules` in `appsettings.json`.

---

## Troubleshooting

**"No entities found" during migration:**
*   Did you run `dotnet build`?
*   Check `src/Core/Iskra.Core.Contracts/Constants/PathConstants.cs` to ensure the build path matches your system.
*   Ensure your module project file has the correct output path configuration (handled by `Directory.Build.props`).

**"Table 'users' already exists":**
*   Check if you are mixing `AutoMigrate` with manual SQL scripts.
*   Ensure `__EFMigrationsHistory` table exists in your database.