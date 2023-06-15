using Evidos.Assignment.Users.Models;

namespace Evidos.Assignment.Users;

public class PendingEmailChangeDto
{
    public EmailChangeToken Token { get; set; }
    public UserId UserId  {  get; set; }
    public Email NewEmail { get; set; }

    private PendingEmailChangeDto(
        EmailChangeToken token,
        UserId userId,
        Email newEmail
    )
    {
        Token = token;
        UserId = userId;
        NewEmail = newEmail;
    }

    internal static PendingEmailChangeDto FromModel(PendingEmailChange model)
    {
        return new PendingEmailChangeDto(model.Token, model.UserId, model.NewEmail);
    }
}