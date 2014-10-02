using System;
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

        public Challenge()
        {
        }

        public DTChallenge getDTChallenge()
        {
            return new DTChallenge(this.ChallengeId,
                                   this.Name,
                                   this.Description,
                                   this.IsEnabled,
                                   this.Level,
                                   this.State.Finished,
                                   this.State.CurrentAttempt,
                                   this.State.BestScore,
                                   this.State.LastScore,
                                   this.State.StartDate);
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
            TimeSpan dif = this.Round.EndDate - dateToday;
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
