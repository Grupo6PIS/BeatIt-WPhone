using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.Resources;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail4 : Challenge
    {
        public int[] TimerValues { get; set; }

        public ChallengeDetail4(int challengeId, string name, string colorHex, int level, int maxAttempts)
        {
            ChallengeId = challengeId;
            Name = AppResources.Challenge4_Title;
            ColorHex = colorHex;
            IsEnabled = true;
            Level = level;
            Description = level == 1 ? AppResources.Challenge4_DescriptionTxtBlockText : AppResources.Challenge4_DescriptionHardTxtBlockText;
            MaxAttempt = maxAttempts;
            TimerValues = Level == 1 ? new[] { 2, 4, 6 } : new[] { 1, 2, 3, 4, 5 };
        }

        public ChallengeDetail4() 
        {
            ChallengeId = 4;
            Name = AppResources.Challenge4_Title;
            ColorHex = "#FF647687";
            Description = AppResources.Challenge4_DescriptionTxtBlockText;
            IsEnabled = true;
            Level = 1;
            MaxAttempt = 3;
            TimerValues = new[] { 2, 4, 6 };
        }

        private int CalculateScore(int[] miliseconds)
        {
            var res = 0;
            for (int i = 0; i < miliseconds.Length; i++) 
            {
                if (miliseconds[i] > 0)
                    res = res + 100/miliseconds[i];
            }
            return res * 10;
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
