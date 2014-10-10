using System;
using SQLite;

namespace BeatIt_.AppCode.Datatypes
{
    public class DTStatePersistible
    {
        [PrimaryKey]
        public int Id { get; set; }

        public int RoundId { get; set; }
        public int ChallengeId { get; set; }
        public bool Finished { get; set; }
        public int BestScore { get; set; }
        public int LastScore { get; set; }
        public DateTime StartDate { get; set; }
        public int CurrentAttempt { get; set; }
    }
}
