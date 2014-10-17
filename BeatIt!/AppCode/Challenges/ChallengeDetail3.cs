using System.Collections.Generic;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.Resources;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail3 : Challenge
    {
        private int _countFacebook;
        private int _countSms;

        public ChallengeDetail3(int challengeId, string colorHex, int level, int maxAttempts)
        {
            ChallengeId = challengeId;
            Name = AppResources.Challenge3_Title;
            ColorHex = colorHex;
            IsEnabled = true;
            Level = level;
            Description = level == 1 ? AppResources.Challenge3_DescriptionTxtBlockText : AppResources.Challenge3_DescriptionHardTxtBlockText;
            _countFacebook = 0;
            _countSms = 0;
            MaxAttempt = maxAttempts;
        }

        public ChallengeDetail3() 
        {
            ChallengeId = 3;
            Name = AppResources.Challenge3_Title;
            ColorHex = "#FF3B5998";
            Description = AppResources.Challenge3_DescriptionTxtBlockText;
            IsEnabled = true;
            Level = 1;
            _countFacebook = 0;
            _countSms = 0;
            MaxAttempt = 3;
        }

        public void AddFacebook()
        {
            _countFacebook++;
        }

        public void AddSms()
        {
            _countSms++;
        }


        public int GetCountFacebook()
        {
            return _countFacebook;
        }

        public int GetCountSms()
        {
            return _countSms;
        }

       

        private int CalculateScore()
        {
            if ((Level == 1) && ((_countFacebook >= 1) && (_countSms >= 2)))
            {
                return (_countFacebook + 3*_countSms)*10;
            }
            if ((Level == 2) && ((_countFacebook >= 1) && (_countSms >= 4)))
            {
                return (_countFacebook + 3*_countSms)*10;
            }
            return 0;
        }

        public KeyValuePair<bool, int> CompleteChallenge(bool error)
        {
            var newScore = false;
            var score = State.BestScore;
            State.CurrentAttempt = State.CurrentAttempt + 1;
            if (!error)
            {
                State.LastScore = CalculateScore();
                if (State.LastScore > State.BestScore)
                {
                    State.BestScore = State.LastScore;
                    FacadeController.GetInstance().ShouldSendScore = true;
                    newScore = true;
                }
                score = State.BestScore;
            }
            else
            {
                State.LastScore = 0;
            }
            if (State.CurrentAttempt == MaxAttempt)
            {
                State.Finished = true;
            }
            FacadeController.GetInstance().SaveState(State);
            
            _countFacebook = 0;
            _countSms = 0;

            return new KeyValuePair<bool, int>(newScore,score);
        }

    }
}
