using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public class Location : ValueObject
    {
        public Location(string location)
        {
            if (string.IsNullOrEmpty(location) || string.IsNullOrWhiteSpace(location))
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