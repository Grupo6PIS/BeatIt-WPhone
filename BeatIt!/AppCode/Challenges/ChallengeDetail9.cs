using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.Resources;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail9 : Challenge
    {
        public struct Song
        {
            public int SelectedIndex;
            public string SongName;
            public string[] OptionsName;
        };

        public int TimerValue { get; set; }
        public Song[] Songs { get; set; }

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

            var s1 = new Song { SelectedIndex = 0, SongName = "dog_bark.wav", OptionsName = new[] { "Perro", "Gato", "Vaca" } };
            var s2 = new Song { SelectedIndex = 1, SongName = "dog_bark.wav", OptionsName = new[] { "Gato", "Perro", "Vaca" } };
            var s3 = new Song { SelectedIndex = 2, SongName = "dog_bark.wav", OptionsName = new[] { "Vaca", "Gato", "Perro" } };
            var s4 = new Song { SelectedIndex = 2, SongName = "dog_bark.wav", OptionsName = new[] { "Gato", "Vaca", "Perro" } };
            var s5 = new Song { SelectedIndex = 2, SongName = "dog_bark.wav", OptionsName = new[] { "Vaca", "Gato", "Perro" } };

            Songs = new[] {s1, s2, s3, s4, s5};
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

        private int CalculateScore(int[] scores)
        {
            var res = 0;
            for (var i = 0; i < scores.Length; i++)
            {
                res = res + scores[i];
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
