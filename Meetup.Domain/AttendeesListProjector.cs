using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Meetup.Domain
{
    public class AttendeesListProjector
    {
        public AttendeesListProjector()
        {
        }

        public AttendeesListReadModel Project(AttendeesListReadModel readModel, params object[] events)
        {
            foreach (var ev in events)
            {
                readModel = When(readModel, ev);
            }
            return readModel;
        }

        private AttendeesListReadModel When(AttendeesListReadModel readModel, object ev)
        {
            switch (ev)
            {
                case Events.MeetupCreated created:
                    readModel.MeetupId = created.MeetupId;
                    break;
                case Events.NumberOfSeatsUpdated seats:
                    readModel.MeetupCapacity = seats.NumberOfSeats;
                    break;
                case Events.RSVPAccepted accepted:
                    if (readModel.MeetupFull)
                    {
                        readModel.WaitingList.Add(accepted.MemberId);
                    }
                    else
                    {
                        readModel.MembersGoing.Add(accepted.MemberId);
                    }
                    break;
                case Events.RSVPDeclined declined:
                    readModel.MembersNotGoing.Add(declined.MemberId);

                    if (readModel.MembersGoing.Contains(declined.MemberId))
                    {
                        if (readModel.WaitingList.Count() > 0)
                        {
                            var memberWaiting = readModel.WaitingList.First();
                            readModel.MembersGoing.Add(memberWaiting);
                            readModel.WaitingList.Remove(memberWaiting);
                        }

                        readModel.MembersGoing.Remove(declined.MemberId);
                    }

                    break;
            }

            return readModel;
        }
    }

    public class AttendeesListReadModel
    {
        public Guid MeetupId { get; set; }
        public List<Guid> MembersGoing { get; set; } = new List<Guid>();
        public List<Guid> WaitingList { get; set; } = new List<Guid>();
        public List<Guid> MembersNotGoing { get; set; } = new List<Guid>();
        public int MeetupCapacity { get; set; }
        public bool MeetupFull => MeetupCapacity == MembersGoing.Count();
    }
}