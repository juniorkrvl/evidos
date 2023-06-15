using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Evidos.Assignment.Users.Models;

namespace Evidos.Assignment.Users
{
    public class UserDto
    {
        public UserId Id { get; set; }
        public string Name  {  get; set; }
        public Email Email { get; set; }
        public string Address { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? VerifiedAt { get; private set; }

        private UserDto(
            UserId id,
            string name, 
            Email email,
            string address,
            DateTime createdAt,
            DateTime? verifiedAt
        )
        {
            Id = id;
            Name = name;
            Email = email;
            Address = address;
            CreatedAt = createdAt;
            VerifiedAt = verifiedAt;
        }

        internal static UserDto FromUserViewModel(UserViewModel model)
        {
            return new UserDto(model.Id, model.Name, model.Email, model.Address, model.CreatedAt, model.VerifiedAt);
        }
    }
}
