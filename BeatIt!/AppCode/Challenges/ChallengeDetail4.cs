using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail4 : Challenge
    {
        public int[] TimerValues { get; set; }

        public ChallengeDetail4(int challengeId, string name, int level)
        {
            ChallengeId = challengeId;
            Name = name;
            ColorHex = "#FF647687";
            Description = "En este desafio debes callar al perro presionando el boton 'Cállate!' 3 veces.";
            IsEnabled = true;
            Level = level;
            this.MaxAttempt = 3;
            this.TimerValues = new int[3] { 2, 4, 6 };
        }

        public ChallengeDetail4() 
        {
            this.ChallengeId = 4;
            this.Name = "Callar al Perro";
            this.ColorHex = "#FF647687";
            this.Description = "En este desafio debes callar al perro presionando el boton 'Cállate!' 3 veces.";
            this.IsEnabled = true;
            this.Level = 1;
            this.MaxAttempt = 3;
            this.TimerValues = new int[3] { 2, 4, 6 };
        }

        private int calculateScore(int[] miliseconds)
        {
            int res = 0;
            for (int i = 0; i < miliseconds.Length; i++) 
            {
                if (miliseconds[i] > 0)
                    res = res + 100/miliseconds[i];
            }
            return res * 10;
        }

        public void completeChallenge(int[] miliseconds)
        {
            this.State.CurrentAttempt = this.State.CurrentAttempt + 1;

            this.State.LastScore = this.calculateScore(miliseconds);
            if (this.State.LastScore > this.State.BestScore)
                this.State.BestScore = this.State.LastScore;
    
            if (this.State.CurrentAttempt == this.MaxAttempt)
            {
                this.State.Finished = true;
            }
            bool actualizo = FacadeController.getInstance().saveState(this.State);
        }
    }
}
