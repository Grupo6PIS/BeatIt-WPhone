using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.Resources;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail9 : Challenge
    {
        public int TimerValue { get; set; }

        public ChallengeDetail9(int challengeId, string colorHex, int level, int maxAttempts, bool isEnabled)
        {
            ChallengeId = challengeId;
            Name = AppResources.Challenge9_Title;
            ColorHex = colorHex;
            IsEnabled = isEnabled;
            Level = level;
            Description = level == 1 ? AppResources.Challenge9_DescriptionTxtBlockText : AppResources.Challenge9_DescriptionHardTxtBlockText;
            MaxAttempt = maxAttempts;
            TimerValue = Level == 1 ? 20 : 10;
        }

        public ChallengeDetail9() 
        {
            ChallengeId = 9;
            Name = AppResources.Challenge9_Title;
            ColorHex = "#FFE3C800";
            IsEnabled = true;
            Level = 1;
            Description = AppResources.Challenge9_DescriptionTxtBlockText;
            MaxAttempt = 3;
            TimerValue = 20;
        }

        private int CalculateScore(int[] miliseconds)
        {
            var res = 0;
            for (int i = 0; i < miliseconds.Length; i++)
            {
                if (miliseconds[i] > 0)
                    res = res + miliseconds[i];
            }
            return res;
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
