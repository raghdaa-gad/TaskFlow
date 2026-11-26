namespace TaskFlow.Services
{
    // Thrown when a business rule is violated. Controllers catch it and show the message to the user.
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message) : base(message) { }
    }
}