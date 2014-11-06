using BeatIt_.Resources;
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
        private const int ChallengeId = 5;
        private const double Speed = 15;
        private const int TimeTop = 45;

        private ChallengeDetail5 _currentChallenge;
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
            _currentChallenge =(ChallengeDetail5) FacadeController.GetInstance().GetChallenge(ChallengeId);
            _randomNumber = new Random();
            
            PageTitle.Text = _currentChallenge.Name;
            TextDescription.Text = _currentChallenge.Description;
            StartTimeTextBlock.Text = _currentChallenge.GetDtChallenge()
                .StartTime.ToString(CultureInfo.InvariantCulture);
            ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";
            DurationTextBlock.Text = _currentChallenge.GetDurationString();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Accelerometer.IsSupported)
            {
                MessageBox.Show(AppResources.Challenge_NotSuported);
                Dispatcher.BeginInvoke(delegate
                {
                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/Home.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                });
                return;
            }

            

            _timeCounter = TimeTop;
            _collisionCounter = 0;

            _fraccion = _currentChallenge.Level == 2 ? 0.7 : 0.9;

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

                _currentChallenge.ChanllengeComplete(_collisionCounter);
              
                var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);

            }
            else
            {
                TimerLabel.Text = _timeCounter + " s";
            }
        }

        void acelerometer_ReadingChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Dispatcher.BeginInvoke(() => UpdatePositions(e.SensorReading.Acceleration.X * Speed, e.SensorReading.Acceleration.Y * Speed));


        }

        private void UpdatePositions(double x,double y ){

            _positionBlackX += x;
            _positionBlackY -= y;

            if (_positionBlackX < 0)
            {
                _positionBlackX = 0;
            }
            else if (_positionBlackX > (CanvasPanel.ActualWidth - BlackBall.Width))
            {
                _positionBlackX = CanvasPanel.ActualWidth - BlackBall.Width;
            }

            if (_positionBlackY < 0)
            {
                _positionBlackY = 0;
            }
            else if (_positionBlackY > (CanvasPanel.ActualHeight - BlackBall.Height))
            {
                _positionBlackY = CanvasPanel.ActualHeight - BlackBall.Height;
            }

            Canvas.SetLeft(BlackBall, _positionBlackX);

            Canvas.SetTop(BlackBall, _positionBlackY);

            if (Collision())
            {
                if (BlackBall.Width > 15 && _randomNumber.Next(0,2) == 1)
                {
                    BlackBall.Width = BlackBall.Width * _fraccion;
                    BlackBall.Height = BlackBall.Height * _fraccion;
                }

                _collisionCounter++;
                CounterLabel.Text = _collisionCounter.ToString(CultureInfo.InvariantCulture);
                GenerateRandomPosition();
            }
        }


        private bool Collision()
        {
            var x = _positionBlackX + (BlackBall.Width / 2);
            var y = _positionBlackY + (BlackBall.Height / 2);

            var sumRadius = (BlackBall.Width + RedBall.Width) / 2;

            var module = Math.Sqrt((x - _positionRedX)*(x - _positionRedX) + (y - _positionRedY)*(y - _positionRedY) );

            return (module < sumRadius );
        }

        private void GenerateRandomPosition()
        {
            if (CanvasPanel.ActualWidth == 0)
            {
                CanvasPanel.UpdateLayout();
            }

            var x = _randomNumber.Next(0, (int)(CanvasPanel.ActualWidth - RedBall.Width));
            var y = _randomNumber.Next(0, (int)(CanvasPanel.ActualHeight - RedBall.Height));

            _positionRedX = x + (RedBall.Width / 2);
            _positionRedY = y + (RedBall.Height / 2);

            Canvas.SetLeft(RedBall, x);
            Canvas.SetTop(RedBall, y);
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