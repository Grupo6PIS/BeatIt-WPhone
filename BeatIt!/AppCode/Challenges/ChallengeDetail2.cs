using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail2 : Challenge
    {
        public ChallengeDetail2(int challengeId, string name, string colorHex, int level, int maxAttempts)
        {
            ChallengeId = challengeId;
            Name = name;
            ColorHex = colorHex;
            Description = "Description 2";
            IsEnabled = true;
            Level = level;
            this.MaxAttempt = maxAttempts;
        }


        public ChallengeDetail2() 
        {
            this.ChallengeId = 2;
            this.Name = "Wake Me Up!";
            this.ColorHex = "#FF00aba9";
            this.Description = "Description 2";
            this.IsEnabled = true;
            this.Level = 1;
            this.MaxAttempt = 3;
        }



        public int[] getSecondsToWakeMeUp()
        {
            int[] result;
            if (this.Level == 1)
            {
                result = new int[3];
                result[0] = 3;
                result[1] = 4;
                result[2] = 5;
            }
            else
            {
                result = new int[4];
                result[0] = 3;
                result[1] = 5;
                result[2] = 7;
                result[3] = 9;
            }

            return result;
        }

        public void CompleteChallenge(int cantCorrectWakeUp)
        {
            this.State.LastScore = this.calculatPuntaje(cantCorrectWakeUp);
            if (this.State.LastScore > this.State.BestScore)
                this.State.BestScore = this.State.LastScore;
            this.State.CurrentAttempt = this.State.CurrentAttempt + 1;
            if (this.State.CurrentAttempt == this.MaxAttempt)
                this.State.Finished = true;

            bool actualizo;

            // Esto no se si esta bien, como en los testing no tenemos sqlite, si estamos testeando no persistimos.
            if(!FacadeController.GetInstance().GetIsForTesting())
                actualizo = FacadeController.GetInstance().SaveState(this.State);
        }

        public int calculatPuntaje(int cantCorrectWakeUp)
        {
            return cantCorrectWakeUp*20;
        }
    }
}
