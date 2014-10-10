using System;

namespace BeatIt_.AppCode.Datatypes
{
    public class DTChallenge
    {
        public int ChallengeId { get; set; }
        public String ChallengeName { get; set; }
        public String ChallengeDescription { get; set; }
        public bool IsEnabled { get; set; }
        public int ChallengeLevel { get; set; }
        public bool Finished { get; set; }
        public int Attempts { get; set; }
        public int BestScore { get; set; }
        public int LastScore { get; set; }
        public DateTime StartTime { get; set; }

        public DTChallenge(int challengeId,
                           String challengeName,
                           String challengeDescription,
                           bool isEnabled,
                           int challengeLevel,
                           bool finished,
                           int attempts,
                           int bestScore,
                           int lastScore, 
                           DateTime startTime)
        {
            ChallengeId = challengeId;
            ChallengeName = challengeName;
            ChallengeDescription = challengeDescription;
            IsEnabled = isEnabled;
            ChallengeLevel = challengeLevel;
            Finished = finished;
            Attempts = attempts;
            BestScore = bestScore;
            LastScore = lastScore;
            StartTime = startTime;
        }
    }
}
