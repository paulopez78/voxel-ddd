using System;
using System.Collections.Generic;
using Xunit;

namespace Meetup.Domain.Tests
{
    public class MeetupStateTests
    {

        [Theory]
        [MemberData(nameof(AllowedTransitions))]
        public void MeetupStateValidTransitionTest(MeetupState from, MeetupState to)
        {
            Assert.True(from.CanTransitionTo(to));
            Assert.Equal(to, from.TransitionTo(to));
        }

        [Theory]
        [MemberData(nameof(NotAllowedTransitions))]
        public void MeetupStateInvalidTransitionTest(MeetupState from, MeetupState to)
        {
            Assert.False(from.CanTransitionTo(to));
            Assert.Throws<ArgumentException>(() => from.TransitionTo(to));
        }
        public static IEnumerable<object[]> AllowedTransitions =>
            new List<object[]>
            {
                new object[] { MeetupState.None, MeetupState.Created},
                new object[] { MeetupState.Created, MeetupState.Published},
            };

        public static IEnumerable<object[]> NotAllowedTransitions =>
            new List<object[]>
            {
                new object[] { MeetupState.Created, MeetupState.Canceled},
                new object[] { MeetupState.Published, MeetupState.Created},
            };
    }
}
