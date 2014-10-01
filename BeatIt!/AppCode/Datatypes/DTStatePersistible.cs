using System;
using SQLite;

namespace BeatIt_.AppCode.Datatypes
{
    public class DTStatePersistible
    {
        [PrimaryKey]
        public int id { get; set; }

        public int roundId { get; set; }
        public int challengeId { get; set; }
        public bool finished { get; set; }
        public int score { get; set; }
        public int lastScore { get; set; }
        public DateTime startDate { get; set; }
        public int currentAttempt { get; set; }
        public int bestTime { get; set; }
    }
}
