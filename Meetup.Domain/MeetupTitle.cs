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

        private MeetupTitle()
        {
            Value = string.Empty;
        }

        public string Value { get; private set; }
        public static MeetupTitle None = new MeetupTitle();

        public static implicit operator string(MeetupTitle title) => title.Value;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public static MeetupTitle From(string title) => new MeetupTitle() { Value = title };
    }
}