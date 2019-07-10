using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public delegate bool AddressValidator(string location);

    public class Location : ValueObject
    {
        public Location(AddressValidator validator, string location)
        {
            if (string.IsNullOrEmpty(location)
                || string.IsNullOrWhiteSpace(location)
                && !validator(location))
                throw new ArgumentException(nameof(location));

            Value = location;
        }

        private Location()
        {
            Value = string.Empty;
        }

        public string Value { get; private set; }
        public static Location None = new Location();

        public static implicit operator string(Location location) => location.Value;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public Location From(string location) => new Location() { Value = location };
    }
}