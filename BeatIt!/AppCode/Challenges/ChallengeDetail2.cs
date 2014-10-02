using BeatIt_.AppCode.Classes;
using BeatIt_.AppCode.Controllers;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail2 : Challenge
    {
        public ChallengeDetail2() 
        {
            this.ChallengeId = 2;
            this.Name = "Wake Me Up!";
            this.ColorHex = "#FF00aba9";
            this.Description = "Description 2";
            this.IsEnabled = true;
            this.Level = 1;
            this.MaxAttempt = 3;
        }



        public int[] getSeconsToWakeMeUp()
        {
            int[] result;
            if (this.Level == 1)
            {
                result = new int[3];
                result[0] = 3;
                result[1] = 4;
                result[2] = 5;
            }
            else
            {
                result = new int[4];
                result[0] = 3;
                result[1] = 5;
                result[2] = 7;
                result[2] = 9;
            }

            return result;
        }



        public void completeChallenge(int cantCorrectWakeUp)
        {
            this.State.setScore(this.calculatPuntaje(cantCorrectWakeUp));     // ACTUALIZAMOS EL PUNTAJE
            this.State.setCurrentAttempt(this.State.getCurrentAttempt() + 1); // INCREMENTAMOS LOS INTENTOS
            this.State.setLastScore(this.calculatPuntaje(cantCorrectWakeUp));
            if (this.State.getCurrentAttempt() == this.MaxAttempt)            // SI YA ALCANZAMOS EL NUMERO MÁXIMO DE INTENTOS, DAMOS EL DESAFIO POR FINALIZADO.
                this.State.setFinished(true);

            bool actualizo = FacadeController.getInstance().saveState(this.State);
        }



        public int calculatPuntaje(int cantCorrectWakeUp)
        {
            return cantCorrectWakeUp*20;
        }
    }
}
