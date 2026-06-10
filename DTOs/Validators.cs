using FluentValidation;

namespace PollApp.Backend.DTOs
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Passwords do not match.");
        }
    }

    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class CreatePollDtoValidator : AbstractValidator<CreatePollDto>
    {
        public CreatePollDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Description).MaximumLength(1000);
            RuleFor(x => x.StartDate).LessThan(x => x.EndDate).WithMessage("Start date must be before end date.");
            RuleFor(x => x.Options).NotNull().Must(x => x.Count >= 2 && x.Count <= 10).WithMessage("A poll must have between 2 and 10 options.");
        }
    }

    public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
    {
        public CreateCommentDtoValidator()
        {
            RuleFor(x => x.CommentText).NotEmpty().MaximumLength(500);
        }
    }
}
