using AbySalto.Mid.Application.Requests;
using AbySalto.Mid.Application.Requests.Basket;
using FluentValidation;

namespace AbySalto.Mid.WebApi.Validation;

public sealed class AddToBasketRequestValidator : AbstractValidator<AddToBasketRequest>
{
    public AddToBasketRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}