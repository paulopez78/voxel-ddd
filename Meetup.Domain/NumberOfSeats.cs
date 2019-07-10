using System;
using System.Collections.Generic;

namespace Meetup.Domain
{
    public class NumberOfSeats : ValueObject
    {
        public NumberOfSeats(int seats)
        {
            if (seats < 1 || seats > 1000) throw new ArgumentException(nameof(seats));
            Value = seats;
        }
        private NumberOfSeats() { }

        public int Value { get; private set; } = 0;

        public static NumberOfSeats None = new NumberOfSeats();

        public static implicit operator int(NumberOfSeats number) => number.Value;

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }

        public static NumberOfSeats From(int numberOfSeats) => new NumberOfSeats { Value = numberOfSeats };
    }
}