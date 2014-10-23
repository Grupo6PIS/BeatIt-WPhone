using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.Resources;

/*****************************/
//SELFIE GROUP
/*****************************/

namespace BeatIt_.AppCode.Challenges
{
    internal class ChallengeDetail10 : Challenge
    {
        public ChallengeDetail10(int challengeId, string colorHex, int level, int maxAttempts, bool isEnabled)
        {
            ChallengeId = challengeId;
            Name = AppResources.Challenge10_Title;
            ColorHex = colorHex;
            IsEnabled = isEnabled;
            Level = level;
            Description = level == 1
                ? AppResources.Challenge10_DescriptionTxtBlockText
                : AppResources.Challenge10_DescriptionHardTxtBlockText;
            MaxAttempt = maxAttempts;
        }

        public ChallengeDetail10()
        {
            ChallengeId = 10;
            Name = AppResources.Challenge10_Title;
            ColorHex = "#FFFA6800";
            IsEnabled = true;
            Level = 1;
            Description = AppResources.Challenge10_DescriptionTxtBlockText;
            MaxAttempt = 3;
        }


        private int CalculateScore(int cantPersonas)
        {
            var points = 0;
            var needed = (Level == 1) ? 2 : 5;
            if (((Level == 1) && (cantPersonas >= needed)) || ((Level == 2) && (cantPersonas >= needed)))
            {
                points = 60 + (cantPersonas - needed)*(60/needed);
            }
            return (points >= 0) ? points : 0;
        }

        public void CompleteChallenge(int cantPersonas)
        {
            State.CurrentAttempt = State.CurrentAttempt + 1;

            State.LastScore = CalculateScore(cantPersonas);
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