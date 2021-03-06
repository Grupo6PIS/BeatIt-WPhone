﻿using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.Resources;

/*****************************/
//SHUT THE DOG!
/*****************************/

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail4 : Challenge
    {
        public ChallengeDetail4(int challengeId, string colorHex, int level, int maxAttempts, bool isEnabled)
        {
            ChallengeId = challengeId;
            Name = AppResources.Challenge4_Title;
            ColorHex = colorHex;
            IsEnabled = isEnabled;
            Level = level;
            Description = level == 1
                ? AppResources.Challenge4_DescriptionTxtBlockText
                : AppResources.Challenge4_DescriptionHardTxtBlockText;
            MaxAttempt = maxAttempts;
            TimerValues = Level == 1 ? new[] {2, 4, 6} : new[] {1, 2, 3, 4, 5};
        }

        public ChallengeDetail4()
        {
            ChallengeId = 4;
            Name = AppResources.Challenge4_Title;
            ColorHex = "#FF647687";
            IsEnabled = true;
            Level = 1;
            Description = AppResources.Challenge4_DescriptionTxtBlockText;
            MaxAttempt = 3;
            TimerValues = new[] {2, 4, 6};
        }

        public int[] TimerValues { get; set; }

        private int CalculateScore(int[] miliseconds)
        {
            int res = 0;
            for (int i = 0; i < miliseconds.Length; i++)
            {
                if (miliseconds[i] > 0)
                    res = res + 100/miliseconds[i];
            }
            return res*10;
        }

        public void CompleteChallenge(int[] miliseconds)
        {
            State.CurrentAttempt = State.CurrentAttempt + 1;

            State.LastScore = CalculateScore(miliseconds);
            if (State.LastScore > State.BestScore)
            {
                State.BestScore = State.LastScore;
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