namespace Evidos.Assignment.Users.Models
{
    internal class PendingEmailChange
    {
        public EmailChangeToken Token { get; set; }

        public UserId UserId { get; set; }

        public Email NewEmail { get; set; }

        public PendingEmailChange(EmailChangeToken token, UserId userId, Email newEmail)
        {
            Token = token;
            UserId = userId;
            NewEmail = newEmail;
        }
    }
}
