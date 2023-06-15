namespace Evidos.Assignment.Users.Models
{
    internal class UserData
    {
        public UserId UserId { get; set; }
        public string Name { get; set; }
        public Email Email { get; set; }
        public string Address { get; set; }


        public UserData(UserId userId, string name, Email email, string address)
        {
            UserId = userId;
            Name = name;
            Email = email;
            Address = address;
        }

        public void UpdateEmail(Email email)
        {
            Email = email;
        }
    }
}
