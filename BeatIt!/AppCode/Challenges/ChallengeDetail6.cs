using BeatIt_.AppCode.Classes;

namespace BeatIt_.AppCode.Challenges
{
    public class ChallengeDetail6 : Challenge
    {
        private double tiempoEnElAire; // En Segundos
        private const double aceleracionGravitatoria = -9.80665f;

        public ChallengeDetail6() 
        {
            this.ChallengeId = 6;
            this.Name = "Flying Phone";
            this.ColorHex = "#FFE61300";
            this.Description = "Description 6";
            this.IsEnabled = true;
            this.Level = 1;
            this.MaxAttempt = 3;
        }

        /// <summary>
        /// Setea la propiedad tiempo en el aire, la misma debe estar medida en segundos.
        /// </summary>
        /// <param name="tiempo"></param>
        public void setTiempoEnElAire(double tiempo)
        {
            this.tiempoEnElAire = tiempo;
        }

        public int CalcularPuntaje()
        {
            double aux = this.tiempoEnElAire / 2;

            double altura = -((aceleracionGravitatoria * aux * aux) / 2);

            // Por ahora retornamos los centimetros que subio.
            return Convert.ToInt32(Math.Round(altura * 100, 0));
        }
    }
}
