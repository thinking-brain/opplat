# C# Coding Instructions

## Target Framework & Language Version

- Target **.NET 10** and **C# 14**.
- Ensure `<TargetFramework>net10.0</TargetFramework>` and `<LangVersion>14</LangVersion>` (or `latest`) in project files.

## Language & Style Guidelines

### Collection Initialization

- Use collection expressions (`[...]`) for initializing arrays, lists, spans, and other collections.

  ```csharp
  // Preferred
  List<string> names = ["Alice", "Bob"];
  int[] ids = [1, 2, 3];

  // Avoid
  var names = new List<string> { "Alice", "Bob" };
  var ids = new int[] { 1, 2, 3 };
  ```

### Records for DTOs

- Use `record` or `record class` for all DTOs (Data Transfer Objects), request/response models, and value objects.
- Prefer positional records for concise immutable DTOs.

  ```csharp
  // Preferred
  public record CreateUserDto(string Name, string Email);
  public record UserResponse(Guid Id, string Name, string Email);

  // Avoid
  public class CreateUserDto
  {
      public string Name { get; init; }
      public string Email { get; init; }
  }
  ```

### Primary Constructors

- Use primary constructors whenever possible for classes and structs (not just records).
- Applies to services, handlers, middleware, and other injectable types.

  ```csharp
  // Preferred
  public class UserService(IUserRepository repository, ILogger<UserService> logger)
  {
      public async Task<UserResponse> GetUserAsync(Guid id)
      {
          // use repository and logger directly
      }
  }

  // Avoid
  public class UserService
  {
      private readonly IUserRepository _repository;
      private readonly ILogger<UserService> _logger;

      public UserService(IUserRepository repository, ILogger<UserService> logger)
      {
          _repository = repository;
          _logger = logger;
      }
  }
  ```
