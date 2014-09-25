using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using BeatIt_.AppCode.Classes;
using System.Device.Location;
using System.Collections.Generic;
using BeatIt_.AppCode.Interfaces;

namespace BeatIt_.AppCode.Challenges
{
    public class UsainBolt : Challenge
    {
        public const double VELOCIDAD_MINIMA = 10;
        public const double TIEMPO_MINIMO_SEG = 30;

        public UsainBolt()
        {
            this.ChallengeId = 1;
            this.Name = "Usain Bolt";
            this.Description = "Se deberrá correr una velocidad minima de " + VELOCIDAD_MINIMA + "Km/h durante " + TIEMPO_MINIMO_SEG.ToString() + " s.";
            this.IsEnabled = true;
            this.Level = 0;
            this.MaxAttempt = 0;

            State state = new State();
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("UsainBoltScore"))
            {
                state.setScore((int)settings["UsainBoltScore"]);
            }
            else
            {
                state.setScore(0);
            }

            if (settings.Contains("UsainBoltBestTime"))
            {
                state.setBestTime((int)settings["UsainBoltBestTime"]);
            }

            this.State = state;
        }

        public int getPuntajeObtenido()
        {
            return this.State.getScore();
        }

        public void finish(int tiempo)
        {
            this.State.setScore(this.calculatePuntaje(tiempo));
            this.State.setFinished(true);
            this.State.setCurrentAttempt(this.State.getCurrentAttempt() + 1);
            
            // FALTARIA GUARDAR EL PUNTAJE.
            
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains("UsainBoltScore"))
            {
                settings.Add("UsainBoltScore", this.State.getScore());
            }

            if (!settings.Contains("UsainBoltBestTime"))
            {
                settings.Add("UsainBoltBestTime", tiempo);
            }

            settings["UsainBoltScore"] = this.State.getScore();
            settings["UsainBoltBestTime"] = tiempo;

            settings.Save();
        }

        private int calculatePuntaje(int timeSpan)
        {
            return 10 + Convert.ToInt32(timeSpan - UsainBolt.TIEMPO_MINIMO_SEG) * 2;
        }
    }
}
