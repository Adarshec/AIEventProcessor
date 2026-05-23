using API.DTOs;
using FluentValidation;

namespace API.Validators;

public class CreateEventRequestValidator
    : AbstractValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
        RuleFor(x => x.DeviceId)
            .NotEmpty();

        RuleFor(x => x.Status)
            .NotEmpty();

        RuleFor(x => x.Temperature)
            .InclusiveBetween(-100, 200);

        RuleFor(x => x.Metadata)
            .NotNull();

        RuleFor(x => x.Metadata.ModelVersion)
            .NotEmpty();

        RuleFor(x => x.Metadata.Confidence)
            .InclusiveBetween(0, 1);
    }
}