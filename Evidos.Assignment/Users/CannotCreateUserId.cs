namespace Evidos.Assignment.Users
{
    public class CannotCreateUserId : UserException
    {
        private CannotCreateUserId(string message) : base(message)
        {}

        public static CannotCreateUserId becauseProvidedIdIsNullEmptyOrWhitespace(string id)
        {
            return new CannotCreateUserId($"Id is null, empty or whitespace. {nameof(id)}");
        }

        public static CannotCreateUserId becauseProvidedIdIsNotGuid(string id)
        {
            return new CannotCreateUserId($"Unable to parse id as Guid.. {nameof(id)}");
        }
    }
}