using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;

/*****************************/
//DESAFIO USAIN BOLT
/*****************************/

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail1 : Challenge
    {

        public int MinSpeed { get; set; }
        public int Time { get; set; }


        public ChallengeDetail1(int challengeId, string name, string colorHex, int level, int maxAttempts)
        {
            ChallengeId = challengeId;
            Name = name;
            ColorHex = colorHex;
            MinSpeed = 10;
            Time = 30;
            Description = "Se deberrá correr una velocidad minima de " + MinSpeed.ToString() + " Km/h durante " + Time.ToString() + " s.";
            IsEnabled = true;
            Level = level;
            MaxAttempt = maxAttempts;
        }

        public ChallengeDetail1() 
        {
            ChallengeId = 1;
            Name = "Usain Bolt";
            ColorHex = "#FF008A00";
            MinSpeed = 10;
            Time = 30;
            Description = "Se deberrá correr una velocidad minima de " + MinSpeed.ToString() + " Km/h durante " + Time.ToString() + " s.";
            IsEnabled = true;
            Level = 1;
            MaxAttempt = 3;
        }

        private int calculateScore(int maxSpeed, int avgSpeed) 
        {
            if ((maxSpeed > 0) && (avgSpeed > 0))
            {
                return 2 * (maxSpeed + avgSpeed);
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
            bool actualizo = FacadeController.getInstance().saveState(this.State);
        }
    }
}
