namespace TaskManager.API.Tests.Validators;

public class CreateTaskRequestValidatorTests
{
    private readonly CreateTaskRequestValidator _validator = new();

    [Theory(DisplayName = "Should return error when the title is empty or null")]
    [Trait("Validator", "CreateTaskRequest")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Should_Return_Error_When_Title_Is_Empty_Or_Null(string invalidTitle)
    {
        var model = TaskFixtures.GetValidCreateTaskRequest() with { Title = invalidTitle };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required");
    }

    [Fact(DisplayName = "Should return error when the title is too short (< 3 characters)")]
    [Trait("Validator", "CreateTaskRequest")]
    public void Should_Return_Error_When_Title_Is_Too_Short()
    {
        var model = TaskFixtures.GetValidCreateTaskRequest() with { Title = "AB" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title must be at least 3 characters");
    }

    [Fact(DisplayName = "Should return error when the title is too long (> 200 characters)")]
    [Trait("Validator", "CreateTaskRequest")]
    public void Should_Return_Error_When_Title_Is_Too_Long()
    {
        var model = TaskFixtures.GetValidCreateTaskRequest() with { Title = new string('a', 201) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title must not exceed 200 characters");
    }

    [Fact(DisplayName = "Should return error when the description is too long (> 1000 characters)")]
    [Trait("Validator", "CreateTaskRequest")]
    public void Should_Return_Error_When_Description_Is_Too_Long()
    {
        var model = TaskFixtures.GetValidCreateTaskRequest() with { Description = new string('a', 1001) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 1000 characters");
    }

    [Fact(DisplayName = "Should not return error when the description is null")]
    [Trait("Validator", "CreateTaskRequest")]
    public void Should_Not_Return_Error_When_Description_Is_Null()
    {
        var model = TaskFixtures.GetValidCreateTaskRequest() with { Description = null };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact(DisplayName = "Should return error when the due date is in the past")]
    [Trait("Validator", "CreateTaskRequest")]
    public void Should_Return_Error_When_DueDate_Is_In_Past()
    {
        var model = TaskFixtures.GetValidCreateTaskRequest() with { DueDate = DateTime.UtcNow.AddMinutes(-1) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DueDate)
            .WithErrorMessage("Due date must be at least 5 minutes in the future");
    }

    [Fact(DisplayName = "Should return error when the due date is more than 1 year in the future")]
    [Trait("Validator", "CreateTaskRequest")]
    public void Should_Return_Error_When_DueDate_Is_Too_Far_In_Future()
    {
        var model = TaskFixtures.GetValidCreateTaskRequest() with { DueDate = DateTime.UtcNow.AddYears(1).AddDays(1) };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DueDate)
            .WithErrorMessage("Due date must be within 1 year");
    }

    [Fact(DisplayName = "Should return error when the user ID is invalid")]
    [Trait("Validator", "CreateTaskRequest")]
    public void Should_Return_Error_When_UserId_Is_Empty()
    {
        var model = TaskFixtures.GetValidCreateTaskRequest() with { UserId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("Invalid User ID format");
    }

    [Theory(DisplayName = "Should not return error when the request is valid")]
    [Trait("Validator", "CreateTaskRequest")]
    [MemberData(nameof(GetValidCreateTaskRequests))]
    public void Should_Not_Return_Error_When_Request_Is_Valid(CreateTaskRequest validRequest)
    {
        var result = _validator.TestValidate(validRequest);
        result.ShouldNotHaveAnyValidationErrors();
    }

    public static IEnumerable<object[]> GetValidCreateTaskRequests()
    {
        for (int i = 0; i < 5; i++)
        {
            yield return new object[] { TaskFixtures.GetValidCreateTaskRequest() };
        }
    }
}