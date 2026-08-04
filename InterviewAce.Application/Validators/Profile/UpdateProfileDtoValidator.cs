using FluentValidation;
using InterviewAce.Application.DTOs.Profile;

namespace InterviewAce.Application.Validators.Profile;

public class UpdateProfileDtoValidator
    : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.")
            .MaximumLength(100)
            .WithMessage("Full name cannot exceed 100 characters.");


        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .WithMessage("Phone number cannot exceed 20 characters.");


        RuleFor(x => x.LinkedInUrl)
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrEmpty(x.LinkedInUrl))
            .WithMessage("Invalid LinkedIn URL.");


        RuleFor(x => x.GitHubUrl)
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrEmpty(x.GitHubUrl))
            .WithMessage("Invalid GitHub URL.");


        RuleFor(x => x.Bio)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Bio))
            .WithMessage("Bio cannot exceed 500 characters.");


        RuleFor(x => x.YearsOfExperience)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Years of experience cannot be negative.");


        RuleFor(x => x.Education)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.Education))
            .WithMessage("Education cannot exceed 200 characters.");
    }


    private bool BeValidUrl(string? url)
    {
        return Uri.TryCreate(
            url,
            UriKind.Absolute,
            out _
        );
    }
}