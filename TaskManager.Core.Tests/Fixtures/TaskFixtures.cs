using Bogus;

namespace TaskManager.Core.Tests.Fixtures;

public static class TaskFixtures
{
    public static Faker<TaskItem> GetTaskFaker()
    {
        return new Faker<TaskItem>()
            .CustomInstantiator(f => new TaskItem(
                title: f.Lorem.Sentence(5),
                description: f.Lorem.Paragraph(),
                dueDate: f.Date.Soon(),
                userId: Guid.NewGuid()
            ))
            .RuleFor(t => t.Id, f => Guid.NewGuid())
            .RuleFor(t => t.CreationDate, f => f.Date.Past());
    }

    public static TaskItem CreateValidTask() => GetTaskFaker().Generate();

    public static List<TaskItem> CreateTaskList(int count) => GetTaskFaker().Generate(count);
}
