using FluentValidation;

namespace TaskManager.API.Models;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Due date is required")
            .GreaterThan(DateTime.UtcNow.AddMinutes(5)).WithMessage("Due date must be at least 5 minutes in the future")
            .LessThan(DateTime.UtcNow.AddYears(1)).WithMessage("Due date must be within 1 year");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required")
            .Must(BeAValidGuid).WithMessage("Invalid User ID format");
    }

    private static bool BeAValidGuid(Guid guid)
    {
        return guid != Guid.Empty;
    }
}
