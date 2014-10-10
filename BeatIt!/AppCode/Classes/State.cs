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
            Finished = false;
            BestScore = 0;
            LastScore = 0;
            StartDate = DateTime.Now;
            CurrentAttempt = 0;
        }

        public DTStatePersistible GetDtStatePersistible()
        {
            var dt = new DTStatePersistible
            {
                Id = Challenge.Round.RoundId*100 + Challenge.ChallengeId,
                RoundId = Challenge.Round.RoundId,
                ChallengeId = Challenge.ChallengeId,
                Finished = Finished,
                LastScore = LastScore,
                BestScore = BestScore,
                StartDate = StartDate,
                CurrentAttempt = CurrentAttempt
            };

            return dt;
        }
    }
}
