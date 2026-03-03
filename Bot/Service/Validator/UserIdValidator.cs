namespace Bot
{
    public class UserIdValidator(long validUserId, long userId, GameResponseFactory factory) : IValidator
    {
        public async Task<ValidationResult> ValidateAsync()
        {
            if (validUserId != userId)
                return new(false, factory.BuildWrongTurnResponse());

            return new(true);
        }
    }
}