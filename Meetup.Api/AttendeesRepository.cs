using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Meetup.Domain;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Meetup.Api
{
    public class AttendeesRepository
    {
        private readonly IMongoCollection<AttendantsMongoDocument> _collection;

        public AttendeesRepository(IConfiguration config)
        {
            var connectionString = config["mongo"] ?? "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            _collection = client.GetDatabase("voxel").GetCollection<AttendantsMongoDocument>("attendants");
        }

        public async Task<AttendeesListReadModel> Get(Guid meetupId)
        {
            var doc = await _collection.Find<AttendantsMongoDocument>(doc => doc.MeetupId == meetupId).FirstOrDefaultAsync();

            if (doc == null)
            {
                return null;
            }

            return new AttendeesListReadModel
            {
                MeetupId = doc.MeetupId,
                MeetupCapacity = doc.MeetupCapacity,
                WaitingList = doc.Waiting,
                MembersGoing = doc.NotGoing,
                MembersNotGoing = doc.Going
            };
        }

        public async Task Save(AttendeesListReadModel readModel)
        {
            await _collection.ReplaceOneAsync(doc => doc.MeetupId == readModel.MeetupId, new AttendantsMongoDocument
            {
                MeetupId = readModel.MeetupId,
                MeetupCapacity = readModel.MeetupCapacity,
                Waiting = readModel.WaitingList,
                NotGoing = readModel.MembersNotGoing,
                Going = readModel.MembersGoing
            }, new UpdateOptions
            {
                IsUpsert = true
            });
        }
    }

    public class AttendantsMongoDocument
    {
        [BsonId]
        public Guid MeetupId { get; set; }
        public int MeetupCapacity { get; set; }
        public List<Guid> Waiting { get; internal set; } = new List<Guid>();
        public List<Guid> Going { get; internal set; } = new List<Guid>();
        public List<Guid> NotGoing { get; internal set; } = new List<Guid>();
    }
}