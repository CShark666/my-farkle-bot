namespace Bot
{
    public record ValidationResult(bool IsValid, BotResponse? Response = null);
    public interface IValidator
    {
        Task<ValidationResult> ValidateAsync();
    }
}