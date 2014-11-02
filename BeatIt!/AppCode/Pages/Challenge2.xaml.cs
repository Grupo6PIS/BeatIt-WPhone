using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
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
            // If the accelerometer is not supported by the device, show a message and return to the list of challenges.
            if (!Accelerometer.IsSupported)
            {
                MessageBox.Show(AppResources.Challenge_NotSuported);
                Dispatcher.BeginInvoke(delegate
                {
                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                });
            }

            // WE GET THE INSTANCE OF CHALLENGE.
            _ifc = FacadeController.GetInstance();
            _currentChallenge = (ChallengeDetail2)_ifc.getChallenge(2);

            // If you already play all attempts, a message is displayed and return to the detail of the challenge
            if (_currentChallenge.State.CurrentAttempt >= _currentChallenge.MaxAttempt)
            {
                // If for some reason enters the challenge with an amount greater than allowed attempts, it is set to the maximum.
                _currentChallenge.State.CurrentAttempt = _currentChallenge.MaxAttempt;
                FacadeController.GetInstance().SaveState(_currentChallenge.State);

                MessageBox.Show(AppResources.Challenge_MaxAttemptsExeeded);
                Dispatcher.BeginInvoke(delegate
                {
                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                });
            }

            // Initialize LABEL DETAIL CHALLENGE
            ShowST.Text = _currentChallenge.GetDtChallenge().StartTime.ToString(CultureInfo.InvariantCulture);
            // Time started the challenge.
            ShowToBeat.Text = _currentChallenge.State.BestScore + " pts"; // Score to beat.
            ShowDuration.Text = _currentChallenge.GetDurationString(); // Challenging time for the challenge.   
            ShowTime.Text = "---"; // Elapsed time.
            textDescription.Text = _currentChallenge.Description;

            // We initialize the TIMER
            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 100) };
            _timer.Tick += TimerTick;

            // We initialize the ACCELEROMETER
            _acelerometro = new Accelerometer();

            // SUCCESS            
            _aciertos = 0;
            _indice = 0;

            // Color Button start.
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

            // If it's time to disappear timer, delete it.
            if (Math.Round((StartTime - tiempoTranscurrido.TotalSeconds), 0) <= _secondsToWakeMeUp[_indice])
            {
                ShowTime.Text = AppResources.Challenge2_Title;
                InProgressGridRectangle.Fill = GetColorFromHexa("#FFe51400");
            }
            else
            {
                ShowTime.Text =
                    Math.Round((StartTime - tiempoTranscurrido.TotalSeconds), 0)
                        .ToString(CultureInfo.InvariantCulture);
                InProgressGridRectangle.Fill = GetColorFromHexa(_currentChallenge.ColorHex);
            }


            if (aux > _finishTime && (aux - _finishTime).TotalMilliseconds > 500) // If I pass the time and not anyone woke, the game ends.
            {
                _indice++;

                FillProgressBar(_indice - 1, true, _currentChallenge.Level);

                if (_indice == _secondsToWakeMeUp.Length) // If already toured all awakened.
                {
                    _timer.Stop();
                    _acelerometro.Stop();
                    _stopwatch.Stop();

                    _currentChallenge.CompleteChallenge(_aciertos);

                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                }
                else // I have to reboot.
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
        /// This method is called when there is a change in the accelerometer data, the same is 
        /// to determine whether the sensed acceleration is significant, in that case:
        /// IF (WAKE ME UP)
        ///     aciertos ++
        ///     IF (there are no attempts to wake up on time)
        ///         finish
        ///     else
        ///         retart with the next attempt.
        /// else
        ///     finish
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acelerometro_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {

            Dispatcher.BeginInvoke(() =>
            {
                var vector = e.SensorReading.Acceleration;

                if ((Math.Abs(Math.Round(Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z), 1)) > 2) && // If I shook the phone enough.
                    _stopwatch.ElapsedMilliseconds > (StartTime - _secondsToWakeMeUp[_indice]) * 1000)  // Y ya no estoy mostrando el cronometro.
                {
                    

                    _acelerometro.Stop();
                    _timer.Stop();
                    _stopwatch.Stop();

                    _indice ++;

                    if (Math.Abs(StartTime*1000 - _stopwatch.ElapsedMilliseconds) <= 500) // If I woke up on time.
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
                    if (_indice == _secondsToWakeMeUp.Length) // If there are no attempts
                    {
                        _currentChallenge.CompleteChallenge(_aciertos);

                        var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                        NavigationService.Navigate(uri);
                    }
                    else // We tried to wake me up the next time.
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

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            _timer.Stop();
            _acelerometro.Stop();

            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Inicializar();
            base.OnNavigatedTo(e);
        }
    }
}