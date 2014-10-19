using BeatIt_.AppCode.Classes;
using BeatIt_.Resources;

namespace BeatIt_.AppCode.Challenges
{
    class ChallengeDetail10 : Challenge
    {
        public ChallengeDetail10(int challengeId, string colorHex, int level, int maxAttempts, bool isEnabled)
        {
            ChallengeId = challengeId;
            Name = AppResources.Challenge10_Title;
            ColorHex = colorHex;
            IsEnabled = isEnabled;
            Level = level;
            Description = level == 1 ? AppResources.Challenge10_DescriptionTxtBlockText : AppResources.Challenge10_DescriptionHardTxtBlockText;
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
    }
}
