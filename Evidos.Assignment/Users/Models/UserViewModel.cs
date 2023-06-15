namespace Evidos.Assignment.Users.Models
{
    public class UserViewModel
    {
        public UserId Id { get; private set; }
        
        public string Name { get; private set; }

        public Email Email { get; private set; }

        public string Address{ get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? VerifiedAt { get; private set; }

        public UserViewModel(UserId id, string name, Email email, string address)
        {
            Id = id;
            Name = name;
            Email = email;
            Address = address;
        }

        public void UpdateEmail(Email email, DateTime verifiedAt)
        {
            Email = email;
            VerifiedAt = verifiedAt;
        }
    }
}
