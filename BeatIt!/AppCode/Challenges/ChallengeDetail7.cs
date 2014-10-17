using BeatIt_.AppCode.Classes;
using BeatIt_.Resources;

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
            Description = level == 1 ? AppResources.Challenge7_DescriptionTxtBlockText : AppResources.Challenge7_DescriptionHardTxtBlockText;
            MaxAttempt = maxAttempts;
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
        }
    }
}
