using System;
using BeatIt_.AppCode.Datatypes;

namespace BeatIt_.AppCode.Classes
{
    public class State
    {
        public bool Finished { get; set; }
        public int BestScore { get; set; }
        public int LastScore { get; set; }
        public DateTime StartDate { get; set; }
        public int CurrentAttempt { get; set; }
        public Challenge Challenge { get; set; }

        public State()
        {
            this.Finished = false;
            this.BestScore = 0;
            this.LastScore = 0;
            this.StartDate = System.DateTime.Now;
            this.CurrentAttempt = 0;
        }

        public DTStatePersistible getDTStatePersistible()
        {
            DTStatePersistible dt = new DTStatePersistible();
            dt.id = this.Challenge.Round.RoundId*100 + this.Challenge.ChallengeId;
            dt.RoundId = this.Challenge.Round.RoundId;
            dt.ChallengeId = this.Challenge.ChallengeId;
            dt.Finished = this.Finished;
            dt.LastScore = this.LastScore;
            dt.BestScore = this.BestScore;
            dt.StartDate = this.StartDate;
            dt.CurrentAttempt = this.CurrentAttempt;

            return dt;
        }
    }
}
