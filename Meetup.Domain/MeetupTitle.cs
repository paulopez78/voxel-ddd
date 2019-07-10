using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public class MeetupTitle : ValueObject
    {
        public MeetupTitle(string title)
        {
            if (string.IsNullOrEmpty(title) || string.IsNullOrWhiteSpace(title))
                throw new ArgumentException(nameof(title));

            Value = title;
        }

        public string Value { get; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}