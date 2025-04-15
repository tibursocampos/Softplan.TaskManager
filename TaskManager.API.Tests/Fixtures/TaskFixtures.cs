namespace TaskManager.API.Tests.Fixtures;

public static class TaskFixtures
{
    private static readonly Faker<CreateTaskRequest> _createTaskRequestFaker = new Faker<CreateTaskRequest>()
        .CustomInstantiator(f => new CreateTaskRequest(
            f.Lorem.Sentence(3, 5).ClampLength(3, 200),
            f.Lorem.Paragraph().ClampLength(0, 1000).OrNull(f, 0.1f),
            f.Date.Future(1, DateTime.UtcNow.AddMinutes(10)),
            f.Random.Guid()
        ));

    private static readonly Faker<TaskItem> _taskItemFaker = new Faker<TaskItem>()
        .CustomInstantiator(f => new TaskItem(
            title: f.Lorem.Sentence(3, 5).ClampLength(3, 200),
            description: f.Lorem.Paragraph().ClampLength(0, 1000).OrNull(f, 0.1f),
            dueDate: f.Date.Future(1, DateTime.UtcNow.AddMinutes(10)),
            userId: f.Random.Guid()
        ))
            .RuleFor(t => t.Id, f => f.Random.Guid())
            .RuleFor(t => t.CreationDate, f => f.Date.Past())
            .RuleFor(t => t.IsCompleted, f => f.Random.Bool(0.2f));

    public static TaskItem GetTaskItemWithSpecifics(
        string title = null,
        string description = null,
        DateTime? dueDate = null,
        Guid? userId = null)
    {
        var faker = new Faker<TaskItem>()
            .CustomInstantiator(f => new TaskItem(
                title: title ?? f.Lorem.Sentence(3, 5).ClampLength(3, 200),
                description: description ?? f.Lorem.Paragraph().ClampLength(0, 1000).OrNull(f, 0.1f),
                dueDate: dueDate ?? f.Date.Future(1, DateTime.UtcNow.AddMinutes(10)),
                userId: userId ?? f.Random.Guid()
        ));

        return faker.Generate();
    }


    public static CreateTaskRequest GetValidCreateTaskRequest()
    {
        return _createTaskRequestFaker.Generate();
    }

    public static CreateTaskRequest GetInvalidCreateTaskRequest()
    {
        return new CreateTaskRequest(
            Title: "A",
            Description: new string('a', 1001),
            DueDate: DateTime.UtcNow.AddMinutes(1),
            UserId: Guid.Empty
        );
    }

    public static List<TaskItem> GetTaskItemsForUser(Guid userId, int count)
    {
        return _taskItemFaker
            .RuleFor(t => t.UserId, userId)
            .Generate(count);
    }
}

public static class BogusExtensions
{
    public static string ClampLength(this string value, int minLength, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;

        value = value.Length > maxLength ? value[..maxLength] : value;
        value = value.Length < minLength ? value.PadRight(minLength, 'x') : value;

        return value;
    }

    public static string OrNull(this string value, Faker f, float chance)
    {
        return f.Random.Float() < chance ? null : value;
    }
}
