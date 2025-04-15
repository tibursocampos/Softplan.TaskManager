# TaskManager API

[![CI](https://github.com/tibursocampos/Softplan.TaskManager/actions/workflows/ci.yml/badge.svg?branch=develop)](https://github.com/tibursocampos/Softplan.TaskManager/actions/workflows/ci.yml)


TaskManager is a .NET API for task management. It includes features like task creation, retrieval, completion, and deletion. The project is developed following clean architecture principles and includes unit and integration tests.

---

## 📚 Table of Contents

- [Features](#features)
- [Project Structure](#project-structure)
- [Libraries and Tools Used](#libraries-and-tools-used)
- [Tests](#tests)
- [Continuous Integration (CI)](#continuous-integration-ci)
- [How to Run](#how-to-run)
- [Initial Data (Seed Data)](#initial-data-seed-data)
- [Examples](#examples)
- [Traceability and Logs](#traceability-and-logs)

---

## Features

- Create, retrieve, update, and delete tasks.
- Logging with Serilog (including Correlation ID support).
- Validation using FluentValidation.
- In-memory database for testing with EF Core.
- Comprehensive unit and integration tests using xUnit.

---

## Project Structure

```text
TaskManager/
├── TaskManager.sln                  # Solution file
├── TaskManager.API                  # API project
├── TaskManager.Core                 # Core business logic
├── TaskManager.Infra                # Infrastructure (data access)
├── TaskManager.Integration.Tests    # Integration tests
├── TaskManager.API.Tests            # API unit tests
├── TaskManager.Core.Tests           # Core unit tests
├── TaskManager.Infra.Tests          # Infra unit tests (repositories)
```
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

## Continuous Integration (CI)

This project uses **GitHub Actions** for continuous integration. The CI pipeline runs automatically on:

- Every pull request targeting the `main` or `dev` branches.
- Manual triggers via the GitHub interface (`workflow_dispatch`).

### CI Steps

The CI workflow performs the following:

1. **Checkout the code** from the repository.
2. **Set up Docker Buildx** for multi-platform builds.
3. **Run all tests** using `docker compose`, ensuring correctness and stability.
4. **Display test coverage summaries** for both the Core, Infra and API projects.
5. **Build the API Docker image** for deployment or further usage.

The workflow is defined in `.github/workflows/ci.yml`.

---

## How to Run

### Local (Visual Studio)

1. Clone the repository.
2. Open the solution `TaskManager.sln` in Visual Studio or any other .NET editor.
3. Run the `TaskManager.API` project.
4. The API will be available at `http://localhost:<port>`.

#### Running the API with Docker Compose

1. Clone the repository.
2. To run only the API in detached mode (background), use the following command:

    ```bash
    docker-compose up -d --build api
    ```

    This will start the API container and expose it on port 8000. You can access the API documentation via Swagger at:

    ```
    http://localhost:8000/swagger
    ```

#### Running Both the API and Tests

1. To run both the API and the tests together, use the following command:

    ```bash
    docker-compose up -d --build
    ```

    This will build and start all containers defined in the `docker-compose.yml` file (API and tests). Both services will run in the background.

#### Running the API with Service Ports

1. Alternatively, you can run the API with:

    ```bash
    docker-compose run --rm --service-ports api
    ```

    This command runs the API service with ports exposed, similar to the `up` command, but it will remove the container once stopped.

#### Running Tests

1. To run the tests in a container, use the following command:

    ```bash
    docker-compose run --rm --service-ports tests
    ```

    This will execute the tests and output the results in the terminal. The container will be removed after execution due to the `--rm` flag.

---

### Considerations

- **Detached Mode (`-d`)**: The `-d` flag runs the containers in the background. You can check logs using `docker-compose logs` or `docker-compose logs <service>` for specific services.
  
- **Service Ports**: The `--service-ports` flag ensures that ports are exposed for the API or tests, making it possible to interact with them as needed.

- **Swagger UI**: The Swagger UI will be available at `http://localhost:8000/swagger` once the API container is running.

- **Container Cleanup**: The `--rm` flag ensures that containers are automatically removed once they are stopped, keeping your environment clean.


---

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
## Traceability and Logs

To facilitate request tracking and debugging, we implemented a structured logging pattern that includes a Correlation ID. This allows tracing the flow of a request across all logs generated during its processing.

### Logging Pattern

Logs follow the format below:

```text
[Timestamp] [Log Level] [Correlation ID] Message
```

* Timestamp: Indicates the date and time when the log was generated.
* Log Level: Indicates the log severity (INFO, WARN, ERROR, etc.).
* Correlation ID: A unique identifier for each request, enabling tracking of all related log messages.
* Message: The actual log message.

### Log Examples

Below is an example of logs generated during the processing of some requests:

```text
2025-04-15 17:50:19 [20:50:19 INF] [23e11ef7] Request: DELETE /api/Tasks/28e2c7e5-3ad3-4a2e-8c71-7b75c8969c5a
2025-04-15 17:50:19 [20:50:19 INF] [23e11ef7] Executing endpoint 'TaskManager.API.Controllers.TasksController.DeleteTask (TaskManager.API)'
2025-04-15 17:50:19 [20:50:19 INF] [23e11ef7] Route matched with {action = "DeleteTask", controller = "Tasks"}. Executing controller action with signature System.Threading.Tasks.Task`1[Microsoft.AspNetCore.Mvc.IActionResult] DeleteTask(System.Guid) on controller TaskManager.API.Controllers.TasksController (TaskManager.API).
2025-04-15 17:50:19 [20:50:19 WRN] [23e11ef7] Task not found for deletion: 28e2c7e5-3ad3-4a2e-8c71-7b75c8969c5a
2025-04-15 17:50:19 [20:50:19 INF] [23e11ef7] Executed action TaskManager.API.Controllers.TasksController.DeleteTask (TaskManager.API) in 46.6494ms
2025-04-15 17:50:19 [20:50:19 INF] [23e11ef7] Executed endpoint 'TaskManager.API.Controllers.TasksController.DeleteTask (TaskManager.API)'
2025-04-15 17:50:19 [20:50:19 ERR] [23e11ef7] Exception: NotFoundException | Status: 404 | Path: /api/Tasks/28e2c7e5-3ad3-4a2e-8c71-7b75c8969c5a | Duration: 72ms
2025-04-15 17:50:19 TaskManager.Core.Exceptions.NotFoundException: Task with id 28e2c7e5-3ad3-4a2e-8c71-7b75c8969c5a not found
```

Note that each request has a unique Correlation ID, making it easy to identify all logs related to a specific request.

