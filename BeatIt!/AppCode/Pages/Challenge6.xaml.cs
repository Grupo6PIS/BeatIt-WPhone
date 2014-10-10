
﻿using System.Globalization;
﻿using Microsoft.Phone.Controls;
﻿using System;
using System.Windows;
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
    public partial class Challenge6
    {
        private ChallengeDetail6 _currentChallenge;    // Instancia del desafio.
        private IFacadeController _ifc;                // Interface del controlador Fachada.

        private bool _isFlying;

        // private DispatcherTimer timer;
        private Accelerometer _accelerometer;

        private readonly Stopwatch _stopwatch = new Stopwatch(); //un objeto tipo Cronómetro


        public Challenge6()
        {
            InitializeComponent();

            var navigateInTransition = new NavigationInTransition
            {
                Backward = new SlideTransition {Mode = SlideTransitionMode.SlideRightFadeIn},
                Forward = new SlideTransition {Mode = SlideTransitionMode.SlideLeftFadeIn}
            };

            var navigateOutTransition = new NavigationOutTransition
            {
                Backward = new SlideTransition {Mode = SlideTransitionMode.SlideRightFadeOut},
                Forward = new SlideTransition {Mode = SlideTransitionMode.SlideLeftFadeOut}
            };
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);

            Inicializar();
        }

        private void Inicializar()
        {
            try
            {
                if (!Accelerometer.IsSupported)
                    throw new Exception("Lamentablemente su dispositivo no tiene las caracteristicas necesarias para jugar este desafio.");

                // OBTENEMOS LA INSTANCIA DEL DESAFIO.
                /* hay que prolijear esto con una factory */
                _ifc = FacadeController.GetInstance();
                _currentChallenge = (ChallengeDetail6)_ifc.getChallenge(6);

                if (_currentChallenge.State.CurrentAttempt == _currentChallenge.MaxAttempt)
                    throw new Exception("Ya ha realizado el número máximo de intentos en la ronda actual.");

                // INICIALIZAMOS LAS ETIQUETAS DEL DETALLE DEL DESAFIO
                ShowST.Text = _currentChallenge.GetDtChallenge().StartTime.ToString(CultureInfo.InvariantCulture); // Tiempo de iniciado el desafio.
                ShowToBeat.Text = _currentChallenge.State.BestScore + " pts";               // Puntaje a vencer.
                ShowDuration.Text = _currentChallenge.GetDurationString();                  // Tiempo retante para realizar el desafio.   
                ShowTime.Text = "---";                                                          // Tiempo transcurrido.

                // INICIALIZAMOS EL ACELEROMETRO.
                _accelerometer = new Accelerometer();

                // EL CELULAR ESTA EN EL AIRE
                _isFlying = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                Dispatcher.BeginInvoke(delegate
                {
                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                });
            }
        }

                
        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("Alerta de que no se pudo cargar la imagen.");
        }


        private void hyperlinkButtonStartRunning_Click(object sender, RoutedEventArgs e)
        {
            StartPlayGrid.Visibility = Visibility.Collapsed;
            //this.InProgressGrid.Visibility = Visibility.Visible;

            _stopwatch.Reset();

            _accelerometer.CurrentValueChanged += _Accelerometer_CurrentValueChanged;
            _accelerometer.TimeBetweenUpdates = new TimeSpan(0, 0, 0, 0, 50);
            _accelerometer.Start();
        }


        /// <summary>
        /// Este metodo al evento CurrentValueChanged del acelerometro, en el mismo se 
        /// llama a la fincion que determinara la accion a tomar.
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _Accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => ProcessAccelerometerReading(e));            
        }


        ///  <summary>
        ///  Este metodo es llamado cuando existe un cambio en los datos del acelerometro, en el mismo se determinara
        ///  si la aceleración captada es considerable, y en dicho caso:
        /// 
        ///  </summary>
        /// <param name="e"></param>
        private void ProcessAccelerometerReading(SensorReadingEventArgs<AccelerometerReading> e)
        {
            try
            {
                Vector3 vector = e.SensorReading.Acceleration;

                if (!_isFlying) // Sie el celular esta en el aire y no habia marcado dicha situacion.
                {
                    if (Math.Abs(Math.Round(Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z), 3)) < 0.2)
                    {
                        _isFlying = true;
                        _stopwatch.Start();
                        PlaySound("/BeatIt!;component/Sounds/dog_bark.wav");
                    }
                }
                else
                {
                    double normaVector = Math.Abs(Math.Round(Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z), 3) - 1);
                    if (normaVector < 0.2) // Si detectamos un vector de fuerza suficiente, y el celular estaba en el aire, es porque ya lo agarraron.
                    {
                        _stopwatch.Stop();
                        _accelerometer.Stop();

                        PlaySound("/BeatIt!;component/Sounds/dog_bark.wav");

                        double tiempo = Convert.ToDouble(_stopwatch.ElapsedMilliseconds) / 1000;

                        _isFlying = false;

                        StartPlayGrid.Visibility = Visibility.Visible;

                        _currentChallenge.State.BestScore = _currentChallenge.CalcularPuntaje(tiempo);
                        _currentChallenge.State.LastScore = _currentChallenge.CalcularPuntaje(tiempo);

                        var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                        NavigationService.Navigate(uri);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void PlaySound(string path)
        {
            StreamResourceInfo stream = Application.GetResourceStream(new Uri(path, UriKind.Relative));
            SoundEffect soundeffect = SoundEffect.FromStream(stream.Stream);
            SoundEffectInstance soundInstance = soundeffect.CreateInstance();
            FrameworkDispatcher.Update();
            soundInstance.Play();
        }
    }
}