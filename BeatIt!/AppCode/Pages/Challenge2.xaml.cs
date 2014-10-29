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
using BeatIt_.Resources;
using Microsoft.Devices.Sensors;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Color = System.Windows.Media.Color;
using Rectangle = System.Windows.Shapes.Rectangle;


/* DESPERTAME A TIEMPO */

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge2
    {
        private const int StartTime = 10;

        private ChallengeDetail2 _currentChallenge;    // Instancia del desafio.
        private IFacadeController _ifc;                // Interface del controlador Fachada.

        private int _aciertos;                         // Cantidad de haciertos.
        private int _indice;

        private int[] _secondsToWakeMeUp;
        private DateTime _finishTime;
        private DateTime _startTime;

        private DispatcherTimer _timer;
        private Accelerometer _acelerometro;

        readonly Stopwatch _stopwatch = new Stopwatch(); //un objeto tipo Cronómetro

        private Rectangle[] _progresRectangles;

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
                MessageBox.Show(AppResources.Challenge_NotSuported);
                Dispatcher.BeginInvoke(delegate
                {
                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                });
            }

            // OBTENEMOS LA INSTANCIA DEL DESAFIO.
            /* hay que prolijear esto con una factory */
            _ifc = FacadeController.GetInstance();
            _currentChallenge = (ChallengeDetail2)_ifc.getChallenge(2);

            // Si ya juegue todos los intentos, muestro un mensaje y vuelvo al detalle del desafio
            if (_currentChallenge.State.CurrentAttempt >= _currentChallenge.MaxAttempt)
            {
                // Si por alguna razon ingresa al desafio con una cantidad de intentos mayor a la permitida, se setea en la mayor.
                _currentChallenge.State.CurrentAttempt = _currentChallenge.MaxAttempt;
                FacadeController.GetInstance().SaveState(_currentChallenge.State);

                MessageBox.Show(AppResources.Challenge_MaxAttemptsExeeded);
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
            textDescription.Text = _currentChallenge.Description;

            // IINICIALIZAMOS EL TIMER
            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 100) };
            _timer.Tick += TimerTick;

            // INICIALIZAMOS EL ACELEROMETRO.
            _acelerometro = new Accelerometer();

            // ACIERTOS            
            _aciertos = 0;
            _indice = 0;

            // Color del Boton start.
            InProgressGridRectangle.Fill = GetColorFromHexa(_currentChallenge.ColorHex);

            // Save the rectangles in array.
            _progresRectangles = new Rectangle[7];
            _progresRectangles[0] = Item1RectangleLevel1;
            _progresRectangles[1] = Item2RectangleLevel1;
            _progresRectangles[2] = Item3RectangleLevel1;
            _progresRectangles[3] = Item1RectangleLevel2;
            _progresRectangles[4] = Item2RectangleLevel2;
            _progresRectangles[5] = Item3RectangleLevel2;
            _progresRectangles[6] = Item4RectangleLevel2;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var tiempoTranscurrido = _stopwatch.Elapsed;
            var aux = _startTime.Add(tiempoTranscurrido);

            // Si es tiempo de desaparecer el timer, lo borramos.
            if (Math.Round((StartTime - tiempoTranscurrido.TotalSeconds), 0) <= _secondsToWakeMeUp[_indice])
            {
                ShowTime.Text = "Wake Me Up!";
                InProgressGridRectangle.Fill = GetColorFromHexa("#FFe51400");
            }
            else
            {
                ShowTime.Text =
                    Math.Round((StartTime - tiempoTranscurrido.TotalSeconds), 0)
                        .ToString(CultureInfo.InvariantCulture);
                InProgressGridRectangle.Fill = GetColorFromHexa(_currentChallenge.ColorHex);
            }


            if (aux > _finishTime && (aux - _finishTime).TotalMilliseconds > 500) // Si me pase del tiempo y no desperte a nadie, termina el juego.
            {
                _indice++;

                FillProgressBar(_indice - 1, true, _currentChallenge.Level);

                if (_indice == _secondsToWakeMeUp.Length) // Si lla recorrimos todas las despertadas.
                {
                    _timer.Stop();
                    _acelerometro.Stop();
                    _stopwatch.Stop();

                    //MessageBox.Show(AppResources.Challenge2_Finish.Replace("@score",
                    //    _currentChallenge.CalculateScore(_aciertos).ToString(CultureInfo.InvariantCulture)));

                    _currentChallenge.CompleteChallenge(_aciertos);

                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                }
                else // Tengo que reiniciar el sistema.
                {
                    RestartIteration();
                }
            }
        }

        private void hyperlinkButtonStartRunning_Click(object sender, RoutedEventArgs e)
        {
            StartPlayGrid.Visibility = Visibility.Collapsed;
            InProgressGrid.Visibility = Visibility.Visible;

            _secondsToWakeMeUp = _currentChallenge.GetSecondsToWakeMeUp();
            _startTime = DateTime.Now;
            _finishTime = _startTime.Add(new TimeSpan(0, 0, StartTime));

            if (_currentChallenge.Level == 1)
            {
                ProgressBarLevel1Grid.Visibility = Visibility.Visible;
                ProgressBarLevel2Grid.Visibility = Visibility.Collapsed;
            }
            else
            {
                ProgressBarLevel1Grid.Visibility = Visibility.Collapsed;
                ProgressBarLevel2Grid.Visibility = Visibility.Visible;                
            }

                // INICIALIZAMOS EL TIMER.
                _timer.Start();
            _stopwatch.Start();

            _acelerometro.CurrentValueChanged += acelerometro_CurrentValueChanged;
            _acelerometro.TimeBetweenUpdates = new TimeSpan(0, 0, 0, 0, 100);
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

                if ((Math.Abs(Math.Round(Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z), 1)) > 2) && // Si Sacudi el celular lo suficiente.
                    _stopwatch.ElapsedMilliseconds > (StartTime - _secondsToWakeMeUp[_indice]) * 1000)  // Y ya no estoy mostrando el ctronometro.
                {
                    

                    _acelerometro.Stop();
                    _timer.Stop();
                    _stopwatch.Stop();

                    _indice ++;

                    if (Math.Abs(StartTime*1000 - _stopwatch.ElapsedMilliseconds) <= 500) // Si me desperto a tiempo.
                    {
                        PlaySound("/BeatIt!;component/Sounds/ring.wav");
                        _aciertos ++;
                        FillProgressBar(_indice - 1, false, _currentChallenge.Level);
                    }
                    else
                    {
                        PlaySound("/BeatIt!;component/Sounds/fail.wav");
                        FillProgressBar(_indice - 1, true, _currentChallenge.Level);
                    }
                    if (_indice == _secondsToWakeMeUp.Length) // Si no quedan despertadas por intentar.
                    {
                        _currentChallenge.CompleteChallenge(_aciertos);

                        // No funciona dentro del begin invoke.
                        //MessageBox.Show("El desafio ha finalizado, ha obtenido " + _currentChallenge.CalculatPuntaje(_aciertos).ToString(CultureInfo.InvariantCulture) + " puntos.");                                                

                        var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                        NavigationService.Navigate(uri);
                    }
                    else // Intentamos con la siguiente despertada.
                        RestartIteration();
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
                Color.FromArgb(
                    Convert.ToByte(hexaColor.Substring(1, 2), 16),
                    Convert.ToByte(hexaColor.Substring(3, 2), 16),
                    Convert.ToByte(hexaColor.Substring(5, 2), 16),
                    Convert.ToByte(hexaColor.Substring(7, 2), 16)
                )
            );
        }

        public void RestartIteration()
        {
            _startTime = DateTime.Now;
            _finishTime = _startTime.Add(new TimeSpan(0, 0, StartTime));
            _timer.Start();
            _stopwatch.Reset();
            _stopwatch.Start();
            _acelerometro.Start();
        }

        public void FillProgressBar(int indice, bool error, int level)
        {
            var mySolidColorBrush = new SolidColorBrush
            {
                Color = error
                    ? Color.FromArgb(255, 255, 20, 0)
                    : Color.FromArgb(255, 0, 138, 0)
            };

            Rectangle r = level == 1 ? _progresRectangles[indice] : _progresRectangles[indice + 3];

            r.Fill = mySolidColorBrush;
        }
    }
}