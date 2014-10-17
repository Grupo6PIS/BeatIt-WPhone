using System;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
/*****************************/
//DESAFIO USAIN BOLT
/*****************************/
using BeatIt_.Resources;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail1 : Challenge
    {

        public int MinSpeed { get; set; }
        public int Time { get; set; }


        public ChallengeDetail1(int challengeId, string colorHex, int level, int maxAttempts)
        {
            ChallengeId = challengeId;
            Name = AppResources.Challenge1_Title;
            ColorHex = colorHex;
            MinSpeed = 10;
            Time = 30;
            IsEnabled = true;
            Level = level;
            MaxAttempt = maxAttempts;
            Description = level == 1 ? AppResources.Challenge1_DescriptionTxtBlockText : AppResources.Challenge1_DescriptionHardTxtBlockText;
        }

        public ChallengeDetail1() 
        {
            ChallengeId = 1;
            Name = AppResources.Challenge1_Title;
            ColorHex = "#FF008A00";
            MinSpeed = 10;
            Time = 30;
            Description = AppResources.Challenge1_DescriptionTxtBlockText;
            IsEnabled = true;
            Level = 1;
            MaxAttempt = 3;
        }

        private int calculateScore(double maxSpeed, double avgSpeed)
        {
            if ((maxSpeed > 0) && (avgSpeed > 0))
            {
                return Convert.ToInt32(Math.Floor(4 * (maxSpeed + avgSpeed)));
            }
            return 0;
        }

        public void CompleteChallenge(bool error, double maxSpeed, double avgSpeed)
        {
            State.CurrentAttempt = State.CurrentAttempt + 1;
            if (!error)
            {
                State.LastScore = calculateScore(maxSpeed, avgSpeed);
                if (State.LastScore > State.BestScore)
                {
                    State.BestScore = State.LastScore;
                    FacadeController.GetInstance().ShouldSendScore = true;
                }
            }
            else 
            {
                State.LastScore = 0;
            }

            if (State.CurrentAttempt == MaxAttempt)
            {
                State.Finished = true;
            }

            // Esto no se si esta bien, como en los testing no tenemos sqlite, si estamos testeando no persistimos.
            if (!FacadeController.GetInstance().GetIsForTesting())
                FacadeController.GetInstance().SaveState(State);
        }
    }
}
