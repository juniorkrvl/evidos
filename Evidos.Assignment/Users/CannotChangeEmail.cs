using Evidos.Assignment.Users.Models;

namespace Evidos.Assignment.Users
{
    public class CannotChangeEmail : UserException
    {
        private CannotChangeEmail(string message) : base(message)
        {}

        public static CannotChangeEmail becauseThereIsNoPendingEmailChangeRequest(EmailChangeToken token) 
        {
            return new CannotChangeEmail($"There is no pending email request for the provided token {token}");
        }
        
        public static CannotChangeEmail becauseTheEmailYouAreTryngToChangeIsAlreadyTheCurrentEmail(Email email) 
        {
            return new CannotChangeEmail($"The email you are trying to change to ({email}) is already the user current's email");
        }
    }
}
