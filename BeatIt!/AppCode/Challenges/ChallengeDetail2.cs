using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.Resources;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail2 : Challenge
    {
        public ChallengeDetail2(int challengeId, string colorHex, int level, int maxAttempts, bool isEnabled)
        {
            ChallengeId = challengeId;
            Name = AppResources.Challenge2_Title;
            ColorHex = colorHex;
            IsEnabled = isEnabled;
            Level = level;
            MaxAttempt = maxAttempts;
            Description = level == 1 ? AppResources.Challenge2_DescriptionTxtBlockText : AppResources.Challenge2_DescriptionHardTxtBlockText;
        }


        public ChallengeDetail2() 
        {
            ChallengeId = 2;
            Name = AppResources.Challenge2_Title;
            ColorHex = "#FF00ABA9";
            Description = AppResources.Challenge2_DescriptionTxtBlockText;
            IsEnabled = true;
            Level = 1;
            MaxAttempt = 3;
        }



        public int[] GetSecondsToWakeMeUp()
        {
            int[] result;
            if (Level == 1)
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
            State.LastScore = CalcularPuntaje(cantCorrectWakeUp);
            if (State.LastScore > State.BestScore)
            {
                State.BestScore = State.LastScore;
                FacadeController.GetInstance().ShouldSendScore = true;
            }
            State.CurrentAttempt = State.CurrentAttempt + 1;
            if (State.CurrentAttempt == MaxAttempt)
                State.Finished = true;

            // Esto no se si esta bien, como en los testing no tenemos sqlite, si estamos testeando no persistimos.
            if(!FacadeController.GetInstance().GetIsForTesting())
                FacadeController.GetInstance().SaveState(State);
        }

        public int CalcularPuntaje(int cantCorrectWakeUp)
        {
            return cantCorrectWakeUp*20;
        }
    }
}
