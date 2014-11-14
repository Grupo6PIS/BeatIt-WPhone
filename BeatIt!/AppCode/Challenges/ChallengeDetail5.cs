using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.Resources;

/*****************************/
//BOUNCING GAME
/*****************************/

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail5 : Challenge
    {
        public ChallengeDetail5(int challengeId, string colorHex, int level, int maxAttempts, bool isEnabled)
        {
            ChallengeId = challengeId;
            Name = AppResources.Challenge5_Title;
            ColorHex = colorHex;
            IsEnabled = isEnabled;
            Level = level;
            Description = level == 1
                ? AppResources.Challenge5_DescriptionTxtBlockText
                : AppResources.Challenge5_DescriptionHardTxtBlockText;
            MaxAttempt = maxAttempts;
        }

        public ChallengeDetail5()
        {
            ChallengeId = 5;
            Name = AppResources.Challenge5_Title;
            ColorHex = "#FFE51400";
            IsEnabled = true;
            Description = AppResources.Challenge5_DescriptionTxtBlockText;
            Level = 1;
            MaxAttempt = 3;
        }

        public void ChanllengeComplete(int collisionCount)
        {
            int puntaje = collisionCount*5;

            State.LastScore = puntaje;
            State.BestScore = puntaje > State.BestScore ? puntaje : State.BestScore;
            State.CurrentAttempt++;

            State.Finished = (State.CurrentAttempt == MaxAttempt);

            if (!FacadeController.GetInstance().GetIsForTesting())
                FacadeController.GetInstance().SaveState(State);
        }
    }
}