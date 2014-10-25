using Microsoft.Phone.Controls;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using System;
using System.Windows.Controls;
using System.Windows;
using System.Globalization;

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge5
    {
        private const int ChallengeId = 5; // segundos
        private const double speed = 15;
        private const int TimeTop = 60; // segundos

        private ChallengeDetail5 _challenge;
        private int _timeCounter, _collisionCounter;
        private double _positionRedX;

        private double _positionRedY;

        private double _positionBlackX;

        private double _positionBlackY;
        private double _fraccion;

        private DispatcherTimer _timer;
        private Accelerometer _acelerometer;
        private Random _randomNumber;

        public Challenge5()
        {
            _positionBlackX = 0;
            _positionBlackY = 0;
            _positionRedX = 0;
            _positionRedY = 0;
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

            InitChallenge();
            
        }

        private void InitChallenge()
        {
            _challenge =(ChallengeDetail5) FacadeController.GetInstance().getChallenge(ChallengeId);
            _randomNumber = new Random();

            StartTimeTextBlock.Text = _challenge.State.StartDate.ToString(CultureInfo.InvariantCulture);
            DurationTextBlock.Text = _challenge.GetDurationString();
            ToBeatTextBlock.Text = _challenge.State.BestScore + "pts";
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Accelerometer.IsSupported)
            {
                MessageBox.Show("Lamentablemente su dispositivo no tiene las caracteristicas necesarias para jugar este desafio.");
                Dispatcher.BeginInvoke(delegate
                {
                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                });
                return;
            }

            if (_challenge.State.CurrentAttempt == _challenge.MaxAttempt)
            {
                MessageBox.Show("Ya ha realizado el número máximo de intentos en la ronda actual.");
                Dispatcher.BeginInvoke(delegate
                {
                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                });

                return;
            }

            _timeCounter = TimeTop;
            _collisionCounter = 0;

            _fraccion = _challenge.Level == 2 ? 0.7 : 0.9;

            _acelerometer = new Accelerometer();
            _acelerometer.CurrentValueChanged += acelerometer_ReadingChanged;
            _timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 1)};
            _timer.Tick += timer_Tick;

            _acelerometer.Start();
            _timer.Start();

            ContentPanel.Visibility = Visibility.Visible;

            GenerateRandomPosition();
        }


        void timer_Tick(object sender, EventArgs e)
        {
            _timeCounter--;

            if (_timeCounter < 0)
            {
                ContentPanel.Visibility = Visibility.Collapsed;

                _timeCounter = 0;
                _timer.Stop();
                _acelerometer.Stop();

                _timer = null;
                _acelerometer = null;

                _challenge.ChanllengeComplete(_collisionCounter);
                MessageBox.Show("Time finished!! You got " + _challenge.State.BestScore + " points");

                var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);

            }
            else
            {
                timerLabel.Text = _timeCounter + "s";
            }
        }

        void acelerometer_ReadingChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Dispatcher.BeginInvoke(() => UpdatePositions(e.SensorReading.Acceleration.X * speed, e.SensorReading.Acceleration.Y * speed));


        }

        private void UpdatePositions(double x,double y ){

            _positionBlackX += x;
            _positionBlackY -= y;

            if (_positionBlackX < 0)
            {
                _positionBlackX = 0;
            }
            else if (_positionBlackX > (canvasPanel.ActualWidth - blackBall.Width))
            {
                _positionBlackX = canvasPanel.ActualWidth - blackBall.Width;
            }

            if (_positionBlackY < 0)
            {
                _positionBlackY = 0;
            }
            else if (_positionBlackY > (canvasPanel.ActualHeight - blackBall.Height))
            {
                _positionBlackY = canvasPanel.ActualHeight - blackBall.Height;
            }

            Canvas.SetLeft(blackBall, _positionBlackX);

            Canvas.SetTop(blackBall, _positionBlackY);

            if (Collision())
            {
                if (blackBall.Width > 10 && _randomNumber.Next(0,2) == 1)
                {
                    blackBall.Width = blackBall.Width * _fraccion;
                    blackBall.Height = blackBall.Height * _fraccion;
                }

                _collisionCounter++;
                counterLabel.Text = _collisionCounter.ToString(CultureInfo.InvariantCulture);
                GenerateRandomPosition();
            }
        }


        private bool Collision()
        {
            var x = _positionBlackX + (blackBall.Width / 2);
            var y = _positionBlackY + (blackBall.Height / 2);

            var sumRadius = (blackBall.Width + redBall.Width) / 2;

            var module = Math.Sqrt((x - _positionRedX)*(x - _positionRedX) + (y - _positionRedY)*(y - _positionRedY) );

            return (module < sumRadius );
        }

        private void GenerateRandomPosition()
        {
            if (canvasPanel.ActualWidth == 0)
            {
                canvasPanel.UpdateLayout();
            }

            var x = _randomNumber.Next(0, (int)(canvasPanel.ActualWidth - redBall.Width));
            var y = _randomNumber.Next(0, (int)(canvasPanel.ActualHeight - redBall.Height));

            _positionRedX = x + (redBall.Width / 2);
            _positionRedY = y + (redBall.Height / 2);

            Canvas.SetLeft(redBall, x);
            Canvas.SetTop(redBall, y);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {

            if (_timer!= null &&  _timer.IsEnabled)
            {
                _timer.Stop();
                _timer = null;
            }

            if (_acelerometer != null)
            {
                _acelerometer.Stop();
                _acelerometer = null;
            }
                
            base.OnBackKeyPress(e);
        }
    }
}