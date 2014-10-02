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
            this.State.CurrentAttempt = this.State.CurrentAttempt + 1;
            if (!error)
            {
                this.State.LastScore = this.calculateScore(miliseconds);
                if (this.State.LastScore > this.State.BestScore)
                    this.State.BestScore = this.State.LastScore;
            }
            else 
            {
                this.State.LastScore = 0;   
            }
            if (this.State.CurrentAttempt == this.MaxAttempt)
            {
                this.State.Finished = true;
            }
        }
    }
}
