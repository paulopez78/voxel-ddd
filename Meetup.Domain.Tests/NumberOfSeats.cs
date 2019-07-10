using System;

namespace Meetup.Domain
{
    public class NumberOfSeats : IEquatable<NumberOfSeats>
    {
        public NumberOfSeats(int seats)
        {
            if (seats < 1 || seats > 1000) throw new ArgumentException(nameof(seats));
            Value = seats;
        }
        private NumberOfSeats() { }

        public int Value { get; } = 0;

        public static NumberOfSeats None = new NumberOfSeats();

        public bool Equals(NumberOfSeats other) =>
            other.Value == Value;
    }
}