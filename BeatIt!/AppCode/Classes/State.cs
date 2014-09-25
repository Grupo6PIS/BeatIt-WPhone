using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BeatIt_.AppCode.Enums;

namespace BeatIt_.AppCode.Classes
{
    public class State
    {
        private bool finished;
        private int score;
        private DateTime startDate;
        private int currentAttempt;
        private int bestTime = 0;
        private Challenge challenge;

        public State()
        {
            this.finished = false;
            this.score = 0;
            this.startDate = System.DateTime.Now;
            this.currentAttempt = 0;
        }

        public void setBestTime(int time)
        {
            if (this.bestTime < time)
            {
                this.bestTime = time;
            }
        }

        public int getBestTime()
        {
            return bestTime;
        }


        public bool getFinished()
        {
            return this.finished;
        }

        public void setFinished(bool finished)
        {
            this.finished = finished;
        }

        public int getScore()
        {
            return this.score;
        }

        public void setScore(int score)
        {
            if (this.score < score)
            {
                this.score = score;
            }
        }

        public DateTime getStartDate()
        {
            return this.startDate;
        }

        public void setStartDate(DateTime startDate)
        {
            this.startDate = startDate;
        }

        public int getCurrentAttempt()
        {
            return this.currentAttempt;
        }

        public void setCurrentAttempt(int attempt)
        {
            this.currentAttempt = attempt;
        }

        public Challenge getChallenge()
        {
            return this.challenge;
        }

        public void setChallenge(Challenge challenge)
        {
            this.challenge = challenge;
        }
    }
}
