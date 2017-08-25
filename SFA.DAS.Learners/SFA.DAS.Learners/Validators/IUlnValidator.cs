namespace SFA.DAS.Learners.Validators
{
    public interface IUlnValidator
    {
        UlnValidationResult Validate(string uln);
    }
}