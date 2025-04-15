# TaskManager API

TaskManager is a .NET API for task management. It includes features like task creation, retrieval, completion, and deletion. The project is developed following clean architecture principles and includes unit and integration tests.

---

## Features

- Create, retrieve, update, and delete tasks.
- Logging with Serilog (including Correlation ID support).
- Validation using FluentValidation.
- In-memory database for testing with EF Core.
- Comprehensive unit and integration tests using xUnit.

---

## Project Structure

TaskManager/
├── TaskManager.sln          # Solution file
├── TaskManager.API          # API project
├── TaskManager.Core         # Core business logic
├── TaskManager.Infra        # Infrastructure (data access)
├── TaskManager.API.Tests    # API unit and integration tests
├── TaskManager.Core.Tests   # Core unit tests
---

## Libraries and Tools Used

- **.NET 8.0**: Framework for building the API.
- **Entity Framework Core (InMemory)**: For database operations and testing.
- **FluentValidation**: For request validation.
- **Serilog**: For structured logging with Correlation ID support.
- **xUnit**: For unit and integration testing.
- **Bogus**: For test data generation.
- **FluentAssertions**: For expressive test assertions.
- **Moq**: For mocking dependencies in tests.

---

## Tests

### Unit Tests

- Located in `TaskManager.API.Tests` and `TaskManager.Core.Tests`.
- Test individual components such as services, controllers, and validators.

### Integration Tests

- Located in `TaskManager.API.Tests/Integration`.
- Test the API endpoints with an in-memory database.

---

## Examples

### Create new task

**Request:**

```json
POST /api/tasks
{
    "title": "Nova Tarefa",
    "description": "Descrição da tarefa",
    "dueDate": "2023-12-31T23:59:59Z",
    "userId": "00000000-0000-0000-0000-000000000001"
}
```
**Response:**

```json
{
    "id": "12345678-1234-1234-1234-123456789012",
    "title": "Nova Tarefa",
    "description": "Descrição da tarefa",
    "creationDate": "2023-01-01T12:00:00Z",
    "dueDate": "2023-12-31T23:59:59Z",
    "isCompleted": false
}
```

## Initial Data (Seed Data)

When the application starts, the database is populated with the following sample data:

-   **User 00000000-0000-0000-0000-000000000001**:
    -   Task: "Implement API", due in 7 days.
    -   Task: "Configure Database", due in 5 days.
    -   Task: "Unit Tests", due in 3 days.
    -   Task: "Initial Deploy", due in 1 day.
-   **User 00000000-0000-0000-0000-000000000002**:
    -   Task: "Documentation", due in 2 days.
    -   Task: "Code Review", due in 4 days.
    -   Task: "Optimize Performance", due in 6 days.
-   **User 00000000-0000-0000-0000-000000000003**:
    -   Task: "Monitoring", due in 1 day.
    -   Task: "Configure CI/CD", due in 2 days.

This data is inserted to facilitate the initial use of the API, demonstrating some existing tasks.

## How to Run

1. Clone the repository.
2. Open the `TaskManager.sln` solution in Visual Studio or a .NET editor.
3. Run the `TaskManager.API` project.
4. The API will be available at `http://localhost:<port>`.