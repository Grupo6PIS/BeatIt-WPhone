using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BeatIt_.AppCode.Enums;

namespace BeatIt_.AppCode.Datatypes
{
    public class DTChallenge
    {
        private int challengeId;
        private String challengeName;
        private String challengeDescription;
        private bool isEnabled;
        private int challengeLevel;
        private bool finished;
        private int attempts;
        private int puntaje;
        private DateTime startTime;
        private int bestTime;

        public DTChallenge(int challengeId,
                           String challengeName,
                           String challengeDescription,
                           bool isEnabled,
                           int challengeLevel,
                           bool finished,
                           int attempts,
                           int puntaje,
                           DateTime startTime,
                           int bestTime)
        {
            this.bestTime = bestTime;
            this.challengeId = challengeId;
            this.challengeName = challengeName;
            this.challengeDescription = challengeDescription;
            this.isEnabled = isEnabled;
            this.challengeLevel = challengeLevel;
            this.finished = finished;
            this.attempts = attempts;
            this.puntaje = puntaje;
            this.startTime = startTime;
        }

        public int getBestTime() { return bestTime; }

        public int getChallengeId() { return this.challengeId; }

        public String getChallengeName() { return this.challengeName; }

        public String getChallengeDescription() { return this.challengeDescription; }

        public bool getIsEnabled() { return this.isEnabled; }

        public int getChallengeLevel() { return this.challengeLevel; }

        public bool getFinished() { return this.finished; }

        public int getAttempts() { return this.attempts; }

        public int getPuntaje() { return this.puntaje; }

        public DateTime getStartTime() { return this.startTime; }
    }
}
