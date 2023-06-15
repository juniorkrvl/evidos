namespace Evidos.Assignment.Users.Models
{
    public class Email: IEquatable<Email>
    {
        private readonly string _email;
        private Email(string email)
        {
            _email = email;
        }

        public static Email FromString(string email)
        {
            return new Email(email);
        }

        public bool Equals(Email? other)
        {
            return other?.ToString() == _email;
        }

        public override string ToString()
        {
            return _email;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Email);
        }

        public override int GetHashCode()
        {
            return _email.GetHashCode();
        }
    }
}


