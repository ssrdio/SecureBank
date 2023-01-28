using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Ctf.CTFd.Models
{
    public class CTFdChallengeModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxAttempts { get; set; }
        public int Value { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public string State { get; set; }
        public CTFdChallengePerquisites Requirements { get; set; }

        public CTFdChallengeModel(long id, string name, string description, int maxAttempts, int value, string category,
            string type, string state)
        {
            Id = id;
            Name = name;
            Description = description;
            MaxAttempts = maxAttempts;
            Value = value;
            Category = category;
            Type = type;
            State = state;
        }

        public class CTFdChallengePerquisites
        {
            public List<long> Prerequisites { get; set; }

            public CTFdChallengePerquisites(List<long> prerequisites)
            {
                Prerequisites = prerequisites;
            }
        }
    }
}
