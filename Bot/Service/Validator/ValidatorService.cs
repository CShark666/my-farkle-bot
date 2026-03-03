namespace Bot
{
    public class ValidatorService
    {
        public async Task<ValidationResult> ValidationAsync(IEnumerable<IValidator> validators)
        {
            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync();
                if (!result.IsValid)
                    return result;
            }
            return new(true);
        }
    }
}