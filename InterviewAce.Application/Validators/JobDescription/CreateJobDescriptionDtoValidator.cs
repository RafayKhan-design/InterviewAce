using FluentValidation;
using InterviewAce.Application.DTOs.JobDescription;

namespace InterviewAce.Application.Validators.JobDescription;

public class CreateJobDescriptionDtoValidator
    : AbstractValidator<CreateJobDescriptionDto>
{
    public CreateJobDescriptionDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(10000);
    }
}