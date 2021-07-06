using Domain;
using FluentValidation;

namespace Application.Activities
{
  
  /**
   * Class to handle validation for the Activity Entity
   */
  public class ActivityValidator : AbstractValidator<Activity>
  {
    public ActivityValidator()
    {
      RuleFor(x => x.Title).NotEmpty();
      RuleFor(x => x.Description).NotEmpty();
      RuleFor(x => x.Date).NotEmpty();
      RuleFor(x => x.Category).NotEmpty();
      RuleFor(x => x.City).NotEmpty();
      RuleFor(x => x.Venue).NotEmpty();
    }
  }
}