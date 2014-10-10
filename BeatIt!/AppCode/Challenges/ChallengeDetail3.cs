using System.Collections.Generic;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail3 : Challenge
    {
        private int _countFacebook;
        private int _countSms;

        public ChallengeDetail3(int challengeId, string name, string colorHex, int level, int maxAttempts)
        {
            ChallengeId = challengeId;
            Name = name;
            ColorHex = colorHex;
            Description = "You have to invite friends using SMS of Facebook";
            IsEnabled = true;
            Level = level;
            MaxAttempt = maxAttempts;
        }

        public ChallengeDetail3() 
        {
            ChallengeId = 3;
            Name = "Invite Friends";
            ColorHex = "#FF3B5998";
            Description = "You have to invite friends using SMS of Facebook";
            IsEnabled = true;
            Level = 1;
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
                    FacadeController.GetInstance().SetHayCambiosParaEnviar();
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

            return new KeyValuePair<bool, int>(newScore,score);
        }

    }
}
