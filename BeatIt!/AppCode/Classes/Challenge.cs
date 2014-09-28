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
using System.Collections.Generic;
using BeatIt_.AppCode.Datatypes;

namespace BeatIt_.AppCode.Classes
{
    public class Challenge
    {
        private int challengeId;
        private string name;
        private string description;
        private bool isEnabled;
        private int level;
        private int maxAttempt;
        private string colorHex;
        private Round round;
        private State state;

        public Challenge()
        {
        }

        public int ChallengeId
        {
            get { return challengeId; }
            set { challengeId = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public int MaxAttempt
        {
            get { return maxAttempt; }
            set { maxAttempt = value; }
        }

        public string ColorHex
        {
            get { return colorHex; }
            set { colorHex = value; }
        }

        public Round Round
        {
            get { return round; }
            set { round = value; }
        }

        public State State
        {
            get { return state; }
            set { state = value; }
        }

        public DTChallenge getDTChallenge()
        {
            return new DTChallenge(this.challengeId,
                                   this.name,
                                   this.description,
                                   this.isEnabled,
                                   this.level,
                                   this.state.getFinished(),
                                   this.state.getCurrentAttempt(),
                                   this.state.getScore(),
                                   this.state.getStartDate(),
                                   this.state.getBestTime());
        }

        /// <summary>
        /// Retorna un string con el tiempo restante para realizar el desafio.
        /// </summary>
        /// <param name="roundDate">Es el tiempo de finalizacion de la ronda,</param>
        /// <returns></returns>
        public string getDurationString()
        {
            String result = "";
            DateTime dateToday = DateTime.Now;
            TimeSpan dif = this.round.EndDate - dateToday;
            int days = dif.Days;
            if (days > 0)
            {
                result = days + " dias";
            }
            else
            {
                int hours = dif.Hours % 24;
                if (hours > 0)
                {
                    result = hours + " horas";
                }
                else
                {
                    int minutes = dif.Minutes % 60;
                    if (minutes > 0)
                    {
                        result = minutes + " minutos";
                    }
                    else
                    {
                        result = "Menos de un minuto!!";
                    }
                }
            }
            return result;
        }
    }
}
