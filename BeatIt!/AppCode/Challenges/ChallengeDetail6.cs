using System;
using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;
using BeatIt_.Resources;

/*****************************/
//THROW THE PHONE!
/*****************************/

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail6 : Challenge
    {
        private const double GravitationalAcceleration = -9.80665f;
        private readonly double[] _metrosMinimosNivel;

        public ChallengeDetail6(int challengeId, string colorHex, int level, int maxAttempts, bool isEnabled)
        {
            _metrosMinimosNivel = new double[] {1f, 2f};

            ChallengeId = challengeId;
            Name = AppResources.Challenge6_Title;
            ColorHex = colorHex;
            IsEnabled = isEnabled;
            Level = level;
            Description = level == 1
                ? AppResources.Challenge6_DescriptionTxtBlockText
                : AppResources.Challenge6_DescriptionHardTxtBlockText;
            MaxAttempt = maxAttempts;
        }

        public ChallengeDetail6()
        {
            _metrosMinimosNivel = new double[] {1f, 2f};

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
            double aux = tiempoEnElAire/2;

            double altura = -((GravitationalAcceleration*aux*aux)/2);

            // Retornamos Segun el nivel (< altura minima, 0; si no 60 + (altura - altura minima) * 60);
            return altura < _metrosMinimosNivel[Level - 1]
                ? 0
                : (Convert.ToInt32(Math.Round(60 + (altura - _metrosMinimosNivel[Level - 1])*60)));
        }

        public void CompleteChallenge(double airTime)
        {
            State.LastScore = CalcularPuntaje(airTime);
            if (State.LastScore > State.BestScore)
            {
                State.BestScore = State.LastScore;
                FacadeController.GetInstance().ShouldSendScore = true;
            }
            State.CurrentAttempt = State.CurrentAttempt + 1;
            if (State.CurrentAttempt == MaxAttempt)
                State.Finished = true;

            if (!FacadeController.GetInstance().GetIsForTesting())
                FacadeController.GetInstance().SaveState(State);
        }
    }
}