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
            Description = "Se deberrá correr una velocidad minima de " + MinSpeed + " Km/h durante " + Time + " s.";
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
            Description = "Se deberrá correr una velocidad minima de " + MinSpeed + " Km/h durante " + Time + " s.";
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
            return 0;
        }

        public void CompleteChallenge(bool error, int maxSpeed, int avgSpeed)
        {
            State.CurrentAttempt = State.CurrentAttempt + 1;
            if (!error)
            {
                State.LastScore = calculateScore(maxSpeed, avgSpeed);
                if (State.LastScore > State.BestScore)
                {
                    State.BestScore = State.LastScore;
                    FacadeController.GetInstance().ShouldSendScore = true;
                }
            }
            else 
            {
                State.LastScore = 0;
            }
            if (State.CurrentAttempt == MaxAttempt)
            {
                State.Finished = true;
            }
            FacadeController.GetInstance().SaveState(State);
        }
    }
}
