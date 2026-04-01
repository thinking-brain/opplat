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
