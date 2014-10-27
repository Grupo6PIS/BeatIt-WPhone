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
            TimerValues = Level == 1 ? 45 : 30;
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
            TimerValues = 30;
        }

        public int TimerValues;

        private int CalculateScore(int good)
        {
            var score = 0;
            for (var i = 0; i < good; i++)
            {
                if (i < 5)
                {
                    score += 1;
                }
                else if (i < 20)
                {
                    score += 5;
                }
                else
                {
                    score += 10;
                }
            }   
            return score;
        }

        public void CompleteChallenge(int good)
        {
            State.CurrentAttempt = State.CurrentAttempt + 1;

            State.LastScore = CalculateScore(good);
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