using BeatIt_.AppCode.Classes;
using System;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail6 : Challenge
    {
        private const double GravitationalAcceleration = -9.80665f;

        public ChallengeDetail6() 
        {
            ChallengeId = 6;
            Name = "Flying Phone";
            ColorHex = "#FFAA00FF";
            Description = "Description 6";
            IsEnabled = true;
            Level = 1;
            MaxAttempt = 3;
        }

        public int CalcularPuntaje(double tiempoEnElAire)
        {
            double aux = tiempoEnElAire / 2;

            double altura = -((GravitationalAcceleration * aux * aux) / 2);

            // Por ahora retornamos los centimetros que subio.
            return Convert.ToInt32(Math.Round(altura * 100, 0));
        }
    }
}
