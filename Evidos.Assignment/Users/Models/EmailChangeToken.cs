namespace Evidos.Assignment.Users.Models
{
    public class EmailChangeToken: IEquatable<EmailChangeToken>
    {
        private readonly Guid _token;

        private EmailChangeToken(Guid guid)
        {
            _token = guid;
        }

        public EmailChangeToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Id is null, empty or whitespace.", nameof(token));
            }

            if (!Guid.TryParse(token, out var parsedId))
            {
                throw new ArgumentException("Unable to parse token as Guid.", nameof(token));
            }

            _token = parsedId;
        }

        public static EmailChangeToken Generate()
        {
            return new EmailChangeToken(Guid.NewGuid());
        }

        public override string ToString() => _token.ToString();

        public bool Equals(EmailChangeToken? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _token.Equals(other._token);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EmailChangeToken)obj);
        }

        public override int GetHashCode()
        {
            return _token.GetHashCode();
        }
    }
}
