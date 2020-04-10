using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureBank.Ctf.CTFd.Models
{
    public class CTFdFlagModel
    {
        public int Id { get; set; }
        public int ChallengeId { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public string Data { get; set; }

        public CTFdFlagModel(int id, int challengeId, string type, string content, string data)
        {
            Id = id;
            ChallengeId = challengeId;
            Type = type;
            Content = content;
            Data = data;
        }
    }
}
