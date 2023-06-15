using Evidos.Assignment.Aggregate;

namespace Evidos.Assignment.Users.Models
{
    public record UserId : IAggregateId, IEquatable<UserId>
    {
        private readonly Guid _id;

        private UserId(Guid guid)
        {
            _id = guid;
        }

        public UserId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw CannotCreateUserId.becauseProvidedIdIsNullEmptyOrWhitespace(id);
            }

            if (!Guid.TryParse(id, out var parsedId))
            {
                throw CannotCreateUserId.becauseProvidedIdIsNotGuid(id);
            }

            _id = parsedId;
        }

        public static UserId Generate()
        {
            return new UserId(Guid.NewGuid());
        }
        public override string ToString() => _id.ToString();
    }
}
