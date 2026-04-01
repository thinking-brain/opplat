# Decisions

## Move Entity Configuration to Fluent API

**Date:** 2026-04-01  
**Author:** Mother (Senior .NET Dev)  
**Status:** Implemented

### Context

The domain entities across the Opplat codebase were heavily annotated with data annotations from System.ComponentModel.DataAnnotations and System.ComponentModel.DataAnnotations.Schema. This violated domain-driven design principles by coupling the domain layer to EF Core infrastructure concerns.

### Decision

Refactor the entire persistence layer to use Fluent API configuration exclusively:

1. **Created IEntityTypeConfiguration<T> classes** for all 42 entities across 5 modules
2. **Organized configurations** in Configurations/ folder with subfolders: Administration/, Sales/, Inventory/, Accounting/, Core/
3. **Removed all EF annotations** from domain entities ([Key], [Required], [MaxLength], [Column], [Table], [ForeignKey], [NotMapped])
4. **Preserved serialization attributes** like [JsonIgnore] as they belong to the serialization concern, not persistence
5. **Updated DbContexts** to use ApplyConfigurationsFromAssembly with namespace filters to maintain bounded context isolation
6. **Generated migrations** for all four DbContexts

### Consequences

#### Positive
- **Clean domain entities**: Domain models are now pure POCOs without infrastructure coupling
- **Centralized configuration**: All EF configuration is in one place per entity
- **Better separation of concerns**: Domain layer no longer depends on EF Core
- **Easier testing**: Domain entities can be tested without EF dependencies
- **Improved maintainability**: Configuration changes don't require touching domain entities

#### Negative
- **More files**: 47 configuration classes added (tradeoff for better organization)
- **Learning curve**: Developers need to know where to find/update entity configurations

### Implementation Notes
- Fixed identity column issue: PostgreSQL only supports UseIdentityByDefaultColumn() on integer types, not GUIDs
- Resolved FK type mismatches using HasPrincipalKey() for alternate key relationships
- Applied namespace filters to prevent cross-module configuration pollution
- All 4 DbContexts successfully generate migrations with the new configuration approach

### Alternatives Considered
1. **Keep data annotations**: Rejected - violates DDD and tight coupling
2. **Mix annotations and Fluent API**: Rejected - inconsistent and confusing
3. **Single large configuration file per DbContext**: Rejected - poor maintainability

### Related
- EF Core Best Practices: https://learn.microsoft.com/en-us/ef/core/modeling/
- DDD Persistence Patterns

---

## TPC Inheritance Strategy for BaseEntity

**Date:** 2026-04-01  
**Author:** Mother  
**Status:** Implemented

### Context

After moving entity configuration to Fluent API, the next step was to standardize the inheritance mapping strategy for all BaseEntity-derived entities. Previous code did not explicitly define a mapping strategy, leaving potential for inconsistent behavior.

### Decision

Adopt TPC (Table-Per-Concrete-Class) as the EF Core inheritance mapping strategy for all `BaseEntity`-derived entities.

### Rationale

- **Performance:** Each concrete entity maps to its own table; no `UNION ALL` queries over discriminator columns, no nullable columns from TPH (Table-Per-Hierarchy)
- **Simplicity:** `BaseEntity` properties (`Id`, `CreatedAt`, `CreatedBy`, `ModifiedAt`, `ModifiedBy`) are defined once in `BaseEntityConfiguration` and applied to all concrete tables
- **Database-Generated IDs:** `gen_random_uuid()` default on `Id` removes the need for app-side UUID generation

### Implementation Details

- **EF Core 10 API:** Use `UseTpcMappingStrategy()` (NOT `UseTpc()` — the shorthand does not exist in EF Core 10)
- **Package Dependency:** `Microsoft.EntityFrameworkCore.Relational` must be an explicit `<PackageReference>` in `Opplat.Infrastructure.csproj` — it is NOT sufficient as a transitive dependency from Npgsql for extension method resolution at compile time
- **Configuration File:** `Configurations/Common/BaseEntityConfiguration.cs` defines the strategy and base property constraints
- **DbContext Updates:** All 4 DbContexts updated to include `Common` namespace in `ApplyConfigurationsFromAssembly` filter

### Changes Made

1. `BaseEntity` → made abstract
2. `User` entity now inherits `BaseEntity`; duplicate `Guid Id` removed
3. `JournalEntry.CreatedBy` (duplicate property hiding base) removed from entity
4. `Configurations/Common/BaseEntityConfiguration` created with `UseTpcMappingStrategy()`
5. All 4 DbContext namespace filters updated to include `Common`
6. `HasKey(e => e.Id)` removed from ~27 BaseEntity-inheriting configurations (redundant under TPC)
7. `TenantConfiguration` cleanups:
   - Removed `Id.HasMaxLength(128)` (unnecessary for UUID columns)
   - Removed `CreatedAt.HasDefaultValue(DateTime.UtcNow)` (static value, incorrect semantics)
8. All 4 migrations deleted and regenerated

### Consequences

#### Positive
- **Clean inheritance model:** Single source of truth for base entity properties and constraints
- **Performance:** No discriminator overhead or nullable columns
- **Schema clarity:** Each table clearly represents a concrete entity type
- **Type safety:** No need for runtime type checks or casting

#### Negative
- Slightly larger storage footprint per concrete table (Id and audit fields repeated)
- Requires developers to understand TPC semantics (acceptable for team familiarity)

### Build Validation

- **Errors:** 0
- **Warnings:** 36 (pre-existing, not related to this change)

### Related Links

- EF Core 10 TPC: https://learn.microsoft.com/en-us/ef/core/modeling/inheritance
- PostgreSQL UUID Functions: https://www.postgresql.org/docs/current/uuid-ossp.html
