using FluentValidation;
using InterviewAce.Application.DTOs.Profile;

namespace InterviewAce.Application.Validators.Profile;

public class CreateProfileDtoValidator
    : AbstractValidator<CreateProfileDto>
{
    public CreateProfileDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100);


        RuleFor(x => x.Phone)
            .MaximumLength(20);


        RuleFor(x => x.LinkedInUrl)
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrEmpty(x.LinkedInUrl));


        RuleFor(x => x.GitHubUrl)
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrEmpty(x.GitHubUrl));
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