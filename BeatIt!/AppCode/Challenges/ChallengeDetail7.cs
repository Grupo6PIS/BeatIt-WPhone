using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.Resources;

/*****************************/
//CATCH ME!
/*****************************/

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail7 : Challenge
    {
        public ChallengeDetail7(int challengeId, string colorHex, int level, int maxAttempts, bool isEnabled)
        {
            ChallengeId = challengeId;
            Name = AppResources.Challenge7_Title;
            ColorHex = colorHex;
            IsEnabled = isEnabled;
            Level = level;
            Description = level == 1
                ? AppResources.Challenge7_DescriptionTxtBlockText
                : AppResources.Challenge7_DescriptionHardTxtBlockText;
            MaxAttempt = maxAttempts;
            TimerValues = Level == 1
                ? new[] {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2}
                : new[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
        }

        public ChallengeDetail7()
        {
            ChallengeId = 7;
            Name = AppResources.Challenge7_Title;
            ColorHex = "#FFD80073";
            IsEnabled = true;
            Level = 1;
            Description = AppResources.Challenge7_DescriptionTxtBlockText;
            MaxAttempt = 3;
            TimerValues = new[] {2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30};
        }

        public int[] TimerValues { get; set; }

        private int CalculateScore(int[] miliseconds)
        {
            int sum = 0;
            int onTime = 0;
            int min = int.MaxValue;
            foreach (int t in miliseconds)
            {
                sum += t;
                if (min > t)
                {
                    min = t;
                }
                if (t > 0)
                {
                    onTime++;
                }
            }
            /*double aux = (sum/miliseconds.Length)/1000;
            return (int)(5*(onTime/(aux+min)));*/
            int mul = (Level == 1) ? 16 : 8;
            int aux = ((onTime*mul) - min) + (sum/miliseconds.Length);
            return aux > 0 ? aux : 0;
        }

        public void CompleteChallenge(int[] miliseconds)
        {
            State.CurrentAttempt = State.CurrentAttempt + 1;

            State.LastScore = CalculateScore(miliseconds);
            if (State.LastScore > State.BestScore)
            {
                State.BestScore = State.LastScore;
                FacadeController.GetInstance().ShouldSendScore = true;
            }

            if (State.CurrentAttempt == MaxAttempt)
            {
                State.Finished = true;
            }

            if (!FacadeController.GetInstance().GetIsForTesting())
                FacadeController.GetInstance().SaveState(State);
        }
    }
}