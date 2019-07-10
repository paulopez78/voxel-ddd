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

        public string Value { get; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}