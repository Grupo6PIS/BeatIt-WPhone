using System.Collections.Generic;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail3 : Challenge
    {
        private int _countFacebook = 0;
        private int _countSMS = 0;

        public ChallengeDetail3(int challengeId, string name, string colorHex, int level, int maxAttempts)
        {
            ChallengeId = challengeId;
            Name = name;
            ColorHex = colorHex;
            Description = "You have to invite friends using SMS of Facebook";
            IsEnabled = true;
            Level = level;
            this.MaxAttempt = maxAttempts;
        }

        public ChallengeDetail3() 
        {
            this.ChallengeId = 3;
            this.Name = "Invite Friends";
            this.ColorHex = "#FF3B5998";
            this.Description = "You have to invite friends using SMS of Facebook";
            this.IsEnabled = true;
            this.Level = 1;
            this.MaxAttempt = 3;
        }

        public void AddFacebook()
        {
            _countFacebook++;
        }

        public void AddSms()
        {
            _countSMS++;
        }

        private int CalculateScore()
        {
            if ((Level == 1) && ((_countFacebook >= 1) && (_countSMS >= 2)))
            {
                return (_countFacebook + 3*_countSMS)*10;
            }
            if ((Level == 2) && ((_countFacebook >= 1) && (_countSMS >= 4)))
            {
                return (_countFacebook + 3*_countSMS)*10;
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
            var actualizo = FacadeController.getInstance().saveState(this.State);

            return new KeyValuePair<bool, int>(newScore,score);
        }

    }
}
