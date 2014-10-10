﻿using System;
using BeatIt_.AppCode.Datatypes;

namespace BeatIt_.AppCode.Classes
{
    public class Challenge
    {
        public int ChallengeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public int Level { get; set; }
        public int MaxAttempt { get; set; }
        public string ColorHex { get; set; }
        public Round Round { get; set; }
        public State State { get; set; }

        public DTChallenge GetDtChallenge()
        {
            return new DTChallenge(ChallengeId,
                                   Name,
                                   Description,
                                   IsEnabled,
                                   Level,
                                   State.Finished,
                                   State.CurrentAttempt,
                                   State.BestScore,
                                   State.LastScore,
                                   State.StartDate);
        }

        /// <summary>
        /// Retorna un string con el tiempo restante para realizar el desafio.
        /// </summary>
        /// <returns></returns>
        public string GetDurationString()
        {
            String result;
            DateTime dateToday = DateTime.Now;
            TimeSpan dif = Round.EndDate - dateToday;
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
