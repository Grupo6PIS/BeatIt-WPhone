using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Resources;
using System.Windows.Threading;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using Microsoft.Devices.Sensors;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;


/* DESPERTAME A TIEMPO */

namespace BeatIt_.AppCode.Pages
{    
    public partial class Challenge2
    {
        private const int FreeTime = 5;

        private ChallengeDetail2 _currentChallenge;    // Instancia del desafio.
        private IFacadeController _ifc;                // Interface del controlador Fachada.
         
        private int _aciertos;                         // Cantidad de haciertos.

        private int[] _secondsToWakeMeUp;
        private DateTime _finishTime;
        private DateTime _startTime;

        private DispatcherTimer _timer;
        private Accelerometer _acelerometro;

        readonly Stopwatch _stopwatch = new Stopwatch(); //un objeto tipo Cronómetro

        public Challenge2() 
        {
            InitializeComponent();

            var navigateInTransition = new NavigationInTransition
            {
                Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeIn },
                Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeIn }
            };

            var navigateOutTransition = new NavigationOutTransition
            {
                Backward = new SlideTransition { Mode = SlideTransitionMode.SlideRightFadeOut },
                Forward = new SlideTransition { Mode = SlideTransitionMode.SlideLeftFadeOut }
            };
            TransitionService.SetNavigationInTransition(this, navigateInTransition);
            TransitionService.SetNavigationOutTransition(this, navigateOutTransition);


            Inicializar();
        }
        
        private void Inicializar()
        {
            // Si el acelerometro no es soportado por el dispositivo, mostramos un mensaje y volvemos al listado de desafios.
            if (!Accelerometer.IsSupported)
            {
                MessageBox.Show("Lamentablemente su dispositivo no tiene las caracteristicas necesarias para jugar este desafio.");
                Dispatcher.BeginInvoke(delegate
                {
                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                });
            }

            // OBTENEMOS LA INSTANCIA DEL DESAFIO.
            /* hay que prolijear esto con una factory */
            _ifc = FacadeController.GetInstance();
            _currentChallenge = (ChallengeDetail2) _ifc.getChallenge(2);

            // Si ya juegue todos los intentos, muestro un mensaje y vuelvo al detalle del desafio
            if (_currentChallenge.State.CurrentAttempt == _currentChallenge.MaxAttempt)
            {
                MessageBox.Show("Ya ha realizado el número máximo de intentos en la ronda actual.");
                Dispatcher.BeginInvoke(delegate
                {
                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                });
            }

            // INICIALIZAMOS LAS ETIQUETAS DEL DETALLE DEL DESAFIO
            ShowST.Text = _currentChallenge.GetDtChallenge().StartTime.ToString(CultureInfo.InvariantCulture);
                // Tiempo de iniciado el desafio.
            ShowToBeat.Text = _currentChallenge.State.BestScore + " pts"; // Puntaje a vencer.
            ShowDuration.Text = _currentChallenge.GetDurationString(); // Tiempo retante para realizar el desafio.   
            ShowTime.Text = "---"; // Tiempo transcurrido.

            // IINICIALIZAMOS EL TIMER
            _timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 1)};
            _timer.Tick += TimerTick;

            // INICIALIZAMOS EL ACELEROMETRO.
            _acelerometro = new Accelerometer();

            // ACIERTOS            
            _aciertos = 0;

            // Color del Boton start.
            InProgressGridRectangle.Fill = GetColorFromHexa(_currentChallenge.ColorHex);
        }
        
        private void TimerTick(object sender, EventArgs e)
        {
            var tiempoTranscurrido = _stopwatch.Elapsed;
            var aux = _startTime.Add(tiempoTranscurrido);

            // Si es tiempo de desaparecer el timer, lo borramos.
            if (!(tiempoTranscurrido.TotalSeconds <= FreeTime))
            {
                ShowTime.Text = "Wake Me Up!";
                InProgressGridRectangle.Fill = GetColorFromHexa("#FFe51400");
            }
            else
            {
                ShowTime.Text =
                    Math.Round((_secondsToWakeMeUp[_aciertos] + FreeTime - tiempoTranscurrido.TotalSeconds), 0)
                        .ToString(CultureInfo.InvariantCulture);
                InProgressGridRectangle.Fill = GetColorFromHexa(_currentChallenge.ColorHex);
            }


            if (aux > _finishTime && (aux - _finishTime).TotalMilliseconds > 500) // Si me pase del tiempo y no desperte a nadie, termina el juego.
            {
                _timer.Stop();
                _acelerometro.Stop();
                _stopwatch.Stop();

                MessageBox.Show("El desafio ha finalizado, ha obtenido " + _currentChallenge.CalculatPuntaje(_aciertos).ToString(CultureInfo.InvariantCulture) + " puntos.");

                _currentChallenge.CompleteChallenge(_aciertos);
                
                var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
            }
        }
        
        private void hyperlinkButtonStartRunning_Click(object sender, RoutedEventArgs e)
        {
            StartPlayGrid.Visibility = Visibility.Collapsed;
            InProgressGrid.Visibility = Visibility.Visible;

            _secondsToWakeMeUp = _currentChallenge.GetSecondsToWakeMeUp();
            _startTime = DateTime.Now;
            _finishTime = _startTime.Add(new TimeSpan(0, 0, _secondsToWakeMeUp[0] + FreeTime));

            // INICIALIZAMOS EL TIMER.
            _timer.Start();
            _stopwatch.Start();

            _acelerometro.CurrentValueChanged += acelerometro_CurrentValueChanged;
            _acelerometro.TimeBetweenUpdates = new TimeSpan(0,0,0,0,100);
            _acelerometro.Start();
        }
        
        /// <summary>
        /// Este metodo es llamado cuando existe un cambio en los datos del acelerometro, en el mismo se determinara
        /// si la aceleración captada es considerable, en dicho caso:
        /// Si (Me desperto a tiempo)
        ///     aciertos ++
        ///     Si (no quedan por despertar)
        ///         finalizar
        ///     else
        ///         volver a inciar con el siguiente a despertar.
        /// else
        ///     finalizar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acelerometro_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {

            //Transladamos los datos a recoger del BackgroundThread y los enviamos al MainThread
            Dispatcher.BeginInvoke(() =>
            {
                var vector = e.SensorReading.Acceleration;
                
                if ((Math.Abs(Math.Round(Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z), 1)) > 2) && _stopwatch.ElapsedMilliseconds > FreeTime * 1000) // Si Sacudi el celular lo suficiente.
                {
                    _acelerometro.Stop();
                    _timer.Stop();
                    _stopwatch.Stop();

                    PlaySound("/BeatIt!;component/Sounds/ring.wav");                    

                    bool finalizar = false;

                    if (Math.Abs(FreeTime * 1000 + _secondsToWakeMeUp[_aciertos] * 1000 - _stopwatch.ElapsedMilliseconds) <= 500) // Si me desperto a tiempo.
                    {
                        _aciertos++;
                        if (_aciertos == _secondsToWakeMeUp.Length)
                            finalizar = true;
                        else
                        {
                            _startTime = DateTime.Now;
                            _finishTime = _startTime.Add(new TimeSpan(0, 0, _secondsToWakeMeUp[_aciertos] + FreeTime));
                            _timer.Start();
                            _stopwatch.Reset();
                            _stopwatch.Start();
                            _acelerometro.Start();
                        }
                    }
                    else
                        finalizar = true;

                    if (finalizar) // Si el desafio finalizo, ya sea porque se equivoco al despertar o porque acerto en todas las despertadas.
                    {
                        _currentChallenge.CompleteChallenge(_aciertos);

                        // No funciona dentro del begin invoke.
                        //MessageBox.Show("El desafio ha finalizado, ha obtenido " + _currentChallenge.CalculatPuntaje(_aciertos).ToString(CultureInfo.InvariantCulture) + " puntos.");                                                

                        var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                        NavigationService.Navigate(uri);
                    }
                }
            });
        }
        
        private void PlaySound(string path)
        {
            StreamResourceInfo stream = Application.GetResourceStream(new Uri(path, UriKind.Relative));
            SoundEffect soundeffect = SoundEffect.FromStream(stream.Stream);
            SoundEffectInstance soundInstance = soundeffect.CreateInstance();
            FrameworkDispatcher.Update();
            soundInstance.Play();
        }
        
        public static SolidColorBrush GetColorFromHexa(string hexaColor)
        {
            return new SolidColorBrush(
                System.Windows.Media.Color.FromArgb(
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16),
                    Convert.ToByte(hexaColor.Substring(7, 2), 16)
                )
            );
        }
    }
}