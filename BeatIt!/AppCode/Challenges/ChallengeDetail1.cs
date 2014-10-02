using BeatIt_.AppCode.Classes;

/*****************************/
//DESAFIO USAIN BOLT
/*****************************/

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail1 : Challenge
    {

        private int minSpeed;
        private int time;

        public int MinSpeed
        {
            get { return minSpeed; }
            set { minSpeed = value; }
        }

        public int Time
        {
            get { return time; }
            set { time = value; }
        }

        public ChallengeDetail1() 
        {
            this.ChallengeId = 1;
            this.Name = "Usain Bolt";
            this.ColorHex = "#FF008A00";
            this.minSpeed = 10;
            this.time = 30;
            this.Description = "Se deberrá correr una velocidad minima de " + this.minSpeed.ToString() + " Km/h durante " + this.time.ToString() + " s.";
            this.IsEnabled = true;
            this.Level = 1;
            this.MaxAttempt = 3;
        }

        private int calculateScore(int maxSpeed, int avgSpeed) 
        {
            if ((maxSpeed > 0) && (avgSpeed > 0))
            {
                return 2 * (maxSpeed + minSpeed);
            }
            else 
            {
                return 0;
            }
        }

        public void completeChallenge(bool error, int maxSpeed, int avgSpeed)
        {
            this.State.CurrentAttempt = this.State.CurrentAttempt + 1;
            if (!error)
            {
                this.State.LastScore = this.calculateScore(maxSpeed, avgSpeed);
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
