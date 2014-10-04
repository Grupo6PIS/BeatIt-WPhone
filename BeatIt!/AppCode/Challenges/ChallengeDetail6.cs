using BeatIt_.AppCode.Classes;
using System;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail6 : Challenge
    {
        private const double aceleracionGravitatoria = -9.80665f;

        public ChallengeDetail6() 
        {
            this.ChallengeId = 6;
            this.Name = "Flying Phone";
            this.ColorHex = "#FFAA00FF";
            this.Description = "Description 6";
            this.IsEnabled = true;
            this.Level = 1;
            this.MaxAttempt = 3;
        }

        public int CalcularPuntaje(double tiempoEnElAire)
        {
            double aux = tiempoEnElAire / 2;

            double altura = -((aceleracionGravitatoria * aux * aux) / 2);

            // Por ahora retornamos los centimetros que subio.
            return Convert.ToInt32(Math.Round(altura * 100, 0));
        }
    }
}
