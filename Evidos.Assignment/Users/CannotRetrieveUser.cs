using Evidos.Assignment.Users.Models;

namespace Evidos.Assignment.Users
{
    internal class CannotRetrieveUser: UserException
    {
        private CannotRetrieveUser(string message) : base(message) { }

        public static CannotRetrieveUser BecauseUserWasNotFound(Email email)
        {
            return new CannotRetrieveUser($"The user with email {email.ToString()} was not found");
        }

        public static CannotRetrieveUser BecauseUserWasNotFound(UserId userId)
        {
            return new CannotRetrieveUser($"The user with id {userId.ToString()} was not found");
        }

        public static CannotRetrieveUser BecausePrivateDataIsNotAvailable(UserId userId)  
        {
            return new CannotRetrieveUser($"The private data for user {userId.ToString()} is not available.");
        }
    }
}
