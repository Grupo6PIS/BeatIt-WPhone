using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail4 : Challenge
    {
        public int[] TimerValues { get; set; }

        public ChallengeDetail4(int challengeId, string name, string colorHex, int level, int maxAttempts)
        {
            ChallengeId = challengeId;
            Name = name;
            ColorHex = colorHex;
            Description = "En este desafio debes callar al perro presionando el boton 'Cállate!' 3 veces.";
            IsEnabled = true;
            Level = level;
            MaxAttempt = maxAttempts;
            TimerValues = Level == 1 ? new[] { 2, 4, 6 } : new[] { 1, 2, 3, 4, 5 };
        }

        public ChallengeDetail4() 
        {
            ChallengeId = 4;
            Name = "Callar al Perro";
            ColorHex = "#FF647687";
            Description = "En este desafio debes callar al perro presionando el boton 'Cállate!' 3 veces.";
            IsEnabled = true;
            Level = 1;
            MaxAttempt = 3;
            TimerValues = Level == 1 ? new[] { 2, 4, 6 } : new[] { 1, 2, 3, 4, 5 };
        }

        private int CalculateScore(int[] miliseconds)
        {
            var res = 0;
            for (int i = 0; i < miliseconds.Length; i++) 
            {
                if (miliseconds[i] > 0)
                    res = res + 100/miliseconds[i];
            }
            return res * 10;
        }

        public void CompleteChallenge(int[] miliseconds)
        {
            State.CurrentAttempt = State.CurrentAttempt + 1;

            State.LastScore = CalculateScore(miliseconds);
            if (State.LastScore > State.BestScore)
            {
                State.BestScore = State.LastScore;
                FacadeController.GetInstance().ShouldSendScore = true;
            }

            if (State.CurrentAttempt == MaxAttempt)
            {
                State.Finished = true;
            }

            // Esto no se si esta bien, como en los testing no tenemos sqlite, si estamos testeando no persistimos.
            if (!FacadeController.GetInstance().GetIsForTesting())
                FacadeController.GetInstance().SaveState(State);
        }
    }
}
