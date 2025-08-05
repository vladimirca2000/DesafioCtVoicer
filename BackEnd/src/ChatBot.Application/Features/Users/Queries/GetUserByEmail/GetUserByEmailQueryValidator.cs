using FluentValidation;

namespace ChatBot.Application.Features.Users.Queries.GetUserByEmail;

/// <summary>
/// Validador para a query GetUserByEmailQuery.
/// </summary>
public class GetUserByEmailQueryValidator : AbstractValidator<GetUserByEmailQuery>
{
    public GetUserByEmailQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail  obrigatrio para a busca.")
            .EmailAddress().WithMessage("O formato do e-mail  invlido.");
    }
}