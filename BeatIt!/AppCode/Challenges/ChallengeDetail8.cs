using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.Resources;

/*****************************/
//COLOR & TEXT
/*****************************/

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail8 : Challenge
    {
        public ChallengeDetail8(int challengeId, string colorHex, int level, int maxAttempts, bool isEnabled)
        {
            ChallengeId = challengeId;
            Name = AppResources.Challenge8_Title;
            ColorHex = colorHex;
            IsEnabled = isEnabled;
            Level = level;
            Description = level == 1
                ? AppResources.Challenge8_DescriptionTxtBlockText
                : AppResources.Challenge8_DescriptionHardTxtBlockText;
            MaxAttempt = maxAttempts;
            TimerValue = Level == 1 ? 60 : 45;
            ColorNamesStrings = new[]
            {
                AppResources.Challenge8_RedColorName, AppResources.Challenge8_BlueColorName,
                AppResources.Challenge8_YellowColorName, AppResources.Challenge8_GreenColorName,
                AppResources.Challenge8_PurpleColorName, AppResources.Challenge8_OrangeColorName
            };
            ColorHexStrings = new[] {"#FFE51400", "#FF0050EF", "#FFE3C800", "#FF008A00", "#FFAA00FF", "#FFFA6800"};
        }

        public ChallengeDetail8()
        {
            ChallengeId = 8;
            Name = AppResources.Challenge8_Title;
            ColorHex = "#FF0050EF";
            IsEnabled = true;
            Level = 1;
            Description = AppResources.Challenge8_DescriptionTxtBlockText;
            MaxAttempt = 3;
            TimerValue = 60;
            ColorNamesStrings = new[]
            {
                AppResources.Challenge8_RedColorName, AppResources.Challenge8_BlueColorName,
                AppResources.Challenge8_YellowColorName, AppResources.Challenge8_GreenColorName,
                AppResources.Challenge8_PurpleColorName, AppResources.Challenge8_OrangeColorName
            };
            ColorHexStrings = new[] {"#FFE51400", "#FF0050EF", "#FFE3C800", "#FF008A00", "#FFAA00FF", "#FFFA6800"};
        }

        public int TimerValue { get; set; }
        public string[] ColorHexStrings { get; set; }
        public string[] ColorNamesStrings { get; set; }

        private int CalculateScore(int hits)
        {
            var res = 0;
            for (var i = 0; i < hits; i++)
            {
                if (i < 5)
                {
                    res = res + 1;
                }
                else
                {
                    if (i < 10)
                    {
                        res = res + 5;
                    }
                    else
                    {
                        res = res + 10;
                    }
                }
            }
            return res;
        }

        public void CompleteChallenge(int hits)
        {
            State.CurrentAttempt = State.CurrentAttempt + 1;

            State.LastScore = CalculateScore(hits);
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