namespace Evidos.Assignment.Users
{
    internal class CannotCreateUser : UserException
    {
        public CannotCreateUser(string message) : base(message) { }
        public static CannotCreateUser becauseEmailAlreadyExists(string email)
        {
            return new CannotCreateUser($"Cannot create user because email {email} already exists in the system");
        }
    }
}
