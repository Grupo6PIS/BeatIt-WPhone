using BeatIt_.AppCode.Classes;
using BeatIt_.Resources;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail9 : Challenge
    {
        public ChallengeDetail9(int challengeId, string colorHex, int level, int maxAttempts)
        {
            ChallengeId = challengeId;
            Name = AppResources.Challenge9_Title;
            ColorHex = colorHex;
            IsEnabled = true;
            Level = level;
            Description = level == 1 ? AppResources.Challenge9_DescriptionTxtBlockText : AppResources.Challenge9_DescriptionHardTxtBlockText;
            MaxAttempt = maxAttempts;
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
        }
    }
}
