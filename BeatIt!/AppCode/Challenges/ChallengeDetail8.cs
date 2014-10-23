using BeatIt_.AppCode.Classes;
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
        }
    }
}