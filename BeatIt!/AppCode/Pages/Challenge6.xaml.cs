
﻿using Microsoft.Phone.Controls;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using System.Diagnostics;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using System.Windows.Resources;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge6 : PhoneApplicationPage
    {
        private ChallengeDetail6 currentChallenge;    // Instancia del desafio.
        private IFacadeController ifc;                // Interface del controlador Fachada.

        private bool estaEnElAire;

        // private DispatcherTimer timer;
        private Accelerometer acelerometro;

        private Stopwatch stopwatch = new Stopwatch(); //un objeto tipo Cronómetro


        public Challenge6()
        {
            InitializeComponent();

            NavigationInTransition navigateInTransition = new NavigationInTransition();
            navigateInTransition.Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeIn };
            navigateInTransition.Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeIn };

            NavigationOutTransition navigateOutTransition = new NavigationOutTransition();
            navigateOutTransition.Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut };
            navigateOutTransition.Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut };
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);

            this.Inicializar();
        }

        private void Inicializar()
        {
            try
            {
                if (!Accelerometer.IsSupported)
                    throw new Exception("Lamentablemente su dispositivo no tiene las caracteristicas necesarias para jugar este desafio.");

                // OBTENEMOS LA INSTANCIA DEL DESAFIO.
                /* hay que prolijear esto con una factory */
                ifc = FacadeController.getInstance();
                this.currentChallenge = (ChallengeDetail6)ifc.getChallenge(6);

                if (this.currentChallenge.State.getCurrentAttempt() == this.currentChallenge.MaxAttempt)
                    throw new Exception("Ya ha realizado el número máximo de intentos en la ronda actual.");

                // INICIALIZAMOS LAS ETIQUETAS DEL DETALLE DEL DESAFIO
                this.ShowST.Text = this.currentChallenge.getDTChallenge().getStartTime().ToString(); // Tiempo de iniciado el desafio.
                this.ShowToBeat.Text = this.currentChallenge.State.getScore() + " pts";              // Puntaje a vencer.
                this.ShowDuration.Text = this.currentChallenge.getDurationString();                  // Tiempo retante para realizar el desafio.   
                this.ShowTime.Text = "---";                                                          // Tiempo transcurrido.

                // INICIALIZAMOS EL ACELEROMETRO.
                this.acelerometro = new Accelerometer();

                // EL CELULAR ESTA EN EL AIRE
                this.estaEnElAire = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                this.Dispatcher.BeginInvoke(delegate()
                {
                    Uri uri = new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                });
            }
        }


        // VER CUAL ES LA IDEA DE ESTA FUNCION????????????????????????????????????????????????
        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }


        private void hyperlinkButtonStartRunning_Click(object sender, RoutedEventArgs e)
        {
            this.StartPlayGrid.Visibility = Visibility.Collapsed;
            //this.InProgressGrid.Visibility = Visibility.Visible;

            this.stopwatch.Reset();

            this.acelerometro.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(acelerometro_CurrentValueChanged);
            this.acelerometro.TimeBetweenUpdates = new TimeSpan(0, 0, 0, 0, 50);
            this.acelerometro.Start();
        }


        /// <summary>
        /// Este metodo es llamado cuando existe un cambio en los datos del acelerometro, en el mismo se determinara
        /// si la aceleración captada es considerable, y en dicho caso:
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acelerometro_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            try
            {
                //Transladamos los datos a recoger del BackgroundThread y los enviamos al MainThread
                Dispatcher.BeginInvoke(() =>
                {
                    Microsoft.Xna.Framework.Vector3 vector = e.SensorReading.Acceleration;

                    if (!this.estaEnElAire) // Sie el celular esta en el aire y no habia marcado dicha situacion.
                    {
                        if (Math.Abs(Math.Round(Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z), 3)) < 0.2)
                        {
                            this.estaEnElAire = true;
                            this.stopwatch.Start();
                            this.PlaySound("/BeatIt!;component/Sounds/dog_bark.wav");
                        }
                    }
                    else
                    {
                        double normaVector = Math.Abs(Math.Round(Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z), 3) - 1);
                        if (normaVector < 0.2) // Si detectamos un vector de fuerza suficiente, y el celular estaba en el aire, es porque ya lo agarraron.
                        {
                            this.stopwatch.Stop();
                            this.acelerometro.Stop();
                            this.PlaySound("/BeatIt!;component/Sounds/dog_bark.wav");                            
                            double tiempo = Convert.ToDouble(this.stopwatch.ElapsedMilliseconds) / 1000;                            
                            this.estaEnElAire = false;
                            this.currentChallenge.setTiempoEnElAire(tiempo);
                            this.StartPlayGrid.Visibility = Visibility.Visible;
                            //this.Start.Text = "Ptos. " + this.currentChallenge.CalcularPuntaje().ToString();                            
                            this.currentChallenge.State.setScore(this.currentChallenge.CalcularPuntaje());
                            this.currentChallenge.State.setLastScore(this.currentChallenge.CalcularPuntaje());
                            Uri uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                            NavigationService.Navigate(uri);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void PlaySound(string path)
        {
            StreamResourceInfo _stream = Application.GetResourceStream(new Uri(path, UriKind.Relative));
            SoundEffect _soundeffect = SoundEffect.FromStream(_stream.Stream);
            SoundEffectInstance soundInstance = _soundeffect.CreateInstance();
            FrameworkDispatcher.Update();
            soundInstance.Play();
        }
    }
}