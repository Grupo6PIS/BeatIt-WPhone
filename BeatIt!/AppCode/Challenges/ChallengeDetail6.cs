using BeatIt_.AppCode.Classes;
using System;
using BeatIt_.Resources;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail6 : Challenge
    {
        private const double GravitationalAcceleration = -9.80665f;

        public ChallengeDetail6(int challengeId, string colorHex, int level, int maxAttempts)
        {
            ChallengeId = challengeId;
            Name = AppResources.Challenge6_Title;
            ColorHex = colorHex;
            IsEnabled = true;
            Level = level;
            Description = level == 1 ? AppResources.Challenge6_DescriptionTxtBlockText : AppResources.Challenge6_DescriptionHardTxtBlockText;
            MaxAttempt = maxAttempts;
        }

        public ChallengeDetail6() 
        {
            ChallengeId = 6;
            Name = AppResources.Challenge6_Title;
            ColorHex = "#FFAA00FF";
            IsEnabled = true;
            Level = 1;
            Description = AppResources.Challenge6_DescriptionTxtBlockText;
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
