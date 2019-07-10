using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public class MemberId : ValueObject
    {
        public MemberId(Guid memberId)
        {
            if (memberId == default)
                throw new ArgumentException(nameof(memberId));

            Value = memberId;
        }
        private MemberId() { }

        public Guid Value { get; private set; }

        public static implicit operator Guid(MemberId id) => id.Value;
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public static MemberId From(Guid memberId) => new MemberId { Value = memberId };
    }
}