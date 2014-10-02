using BeatIt_.AppCode.Classes;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail4 : Challenge
    {
        public ChallengeDetail4() 
        {
            this.ChallengeId = 4;
            this.Name = "Challenge 4";
            this.ColorHex = "#FF647687";
            this.Description = "Description 4";
            this.IsEnabled = true;
            this.Level = 1;
            this.MaxAttempt = 3;
        }

        private int calculateScore(int miliseconds)
        {
            return miliseconds;
        }

        public void completeChallenge(bool error, int miliseconds)
        {
            this.State.setCurrentAttempt(this.State.getCurrentAttempt() + 1);
            if (!error)
            {
                this.State.setScore(this.calculateScore(miliseconds));
            }
            if (this.State.getCurrentAttempt() == this.MaxAttempt)
            {
                this.State.setFinished(true);
            }
        }
    }
}
