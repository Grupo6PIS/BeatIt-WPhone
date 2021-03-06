﻿using System;
using System.ComponentModel;
using System.Device.Location;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using BeatIt_.AppCode.Challenges;
using BeatIt_.AppCode.Controllers;
using BeatIt_.AppCode.Interfaces;
using BeatIt_.Resources;
using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Environment = Microsoft.Devices.Environment;

/* USAIN BOLT */

namespace BeatIt_.AppCode.Pages
{
    public partial class Challenge1
    {
        /******************************************************************************************************************/

        private readonly bool _useEmulation = (Environment.DeviceType == DeviceType.Emulator);

        private ChallengeDetail1 _currentChallenge;
        private GeoCoordinateWatcher _gps;
        private IFacadeController _ifc;

        private Boolean _isRunning;
        private Boolean _finish;

        /******************************************************************************************************************/

        private int _minSpeed, _minTime;
        private double _avgSpeed;
        private double _maxSpeed;
        private int _count;
        private int _seconds;
        private GpsSpeedEmulator _speedEmulator;
        private DispatcherTimer _timer;

        public Challenge1()
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

            InitChallenge();
        }


        private void InitChallenge()
        {
            _ifc = FacadeController.GetInstance();
            _currentChallenge = (ChallengeDetail1)_ifc.GetChallenge(1);

            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            _timer.Tick += TickTemp;

            _minTime = _currentChallenge.Time;
            _minSpeed = _currentChallenge.MinSpeed;
            _seconds = _minTime;
            _finish = false;
            _maxSpeed = 0;
            _avgSpeed = 0;
            _count = 0;

            PageTitle.Text = _currentChallenge.Name;
            TextDescription.Text = _currentChallenge.Description;
            StartTimeTextBlock.Text = _currentChallenge.GetDtChallenge().StartTime.ToString(CultureInfo.InvariantCulture);
            ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";
            DurationTextBlock.Text = _currentChallenge.GetDurationString();

            ShowTime.Text = _minTime.ToString(CultureInfo.InvariantCulture);
            ShowSpeed.Text = "0.00";

            if (_useEmulation)
            {
                _speedEmulator = new GpsSpeedEmulator();
                _speedEmulator.SpeedChange += SpeedChanged;
            }
            else
            {
                StartRunningButton.IsEnabled = false;
                StartRunningRectangle.Opacity = 0.5;

                _gps = new GeoCoordinateWatcher(GeoPositionAccuracy.High) { MovementThreshold = 3 };
                _gps.PositionChanged += PositionChanged;
                _gps.StatusChanged += StatusChanged;
                _gps.Start();
                ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";
            }
        }

        private void TickTemp(object o, EventArgs e)
        {
            if (!_isRunning) return;
            _seconds--;
            if (_seconds == 0)
            {
                if (!_finish)
                {

                    _timer.Stop();
                    _seconds = _minTime;
                    _isRunning = false;

                    ShowTime.Text = "0.00";

                    StartRunningGrid.Visibility = Visibility.Visible;
                    InProgressGrid.Visibility = Visibility.Collapsed;

                    if (_useEmulation)
                    {
                        _speedEmulator.Stop();
                    }
                    _seconds = _minTime;


                    //MessageBox.Show(AppResources.Challenge1_Win);
                    _currentChallenge = (ChallengeDetail1)_ifc.GetChallenge(1);
                    _currentChallenge.CompleteChallenge(false, _maxSpeed, (_avgSpeed / _count));
                    ToBeatTextBlock.Text = _currentChallenge.State.BestScore + " pts";
                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                    _finish = true;
                }
            }
            else
            {
                ShowTime.Text = _seconds.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void UpdateLabels(double speed)
        {
            if (speed <= 0 || double.IsNaN(speed))
            {
                ShowSpeed.Text = "0.00";
            }
            else
            {
                ShowSpeed.Text = String.Format("{0:0.##}", speed);
            }
        }

        private void PositionChanged(object obj, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            double speed = e.Position.Location.Speed * 3.6;

            SpeedChanged(speed);
        }

        private void SpeedChanged(double speed)
        {
            UpdateLabels(speed);

            if (speed > _maxSpeed)
                _maxSpeed = speed;
            _count++;
            _avgSpeed += speed;

            if (speed >= _minSpeed)
            {
                _isRunning = true;
                var mySolidColorBrush = new SolidColorBrush { Color = Color.FromArgb(255, 229, 20, 0) };
                SpeedRec.Fill = mySolidColorBrush;

                if (!_timer.IsEnabled)
                {
                    _timer.Start();
                }
            }
            else
            {
                if (!_finish)
                {
                    _isRunning = false;
                    var mySolidColorBrush = new SolidColorBrush { Color = Color.FromArgb(255, 0, 138, 0) };
                    SpeedRec.Fill = mySolidColorBrush;
                    if (!_timer.IsEnabled) return;

                    _timer.Stop();
                    _seconds = _minTime;

                    _currentChallenge.CompleteChallenge(false, 0, 0);
                    
                    if (_useEmulation)
                    {
                        _speedEmulator.Stop();
                    }

                    var uri = new Uri("/BeatIt!;component/AppCode/Pages/ChallengeDetail.xaml", UriKind.Relative);
                    NavigationService.Navigate(uri);
                    _finish = true;

                    

                    //MessageBox.Show(AppResources.Challenge1_Loss);
                }
            }
        }

        private void StatusChanged(object obj, GeoPositionStatusChangedEventArgs e)
        {
            //String statusType = "";
            if (e.Status == GeoPositionStatus.NoData)
            {
                //statusType = "NoData";
            }
            if (e.Status == GeoPositionStatus.Initializing)
            {
                //statusType = "Initializing";
            }
            if (e.Status == GeoPositionStatus.Ready)
            {
                //statusType = "Ready";
                StartRunningButton.IsEnabled = true;
                StartRunningRectangle.Opacity = 1.0;
            }
            if (e.Status == GeoPositionStatus.Disabled)
            {
                //statusType = "Disabled";
            }
            //this.ShowDuration.Text = "Status: " + statusType;
        }

        private void hyperlinkButtonStartRunning_Click(object sender, RoutedEventArgs e)
        {
            if (_isRunning)
            {
                MessageBox.Show(AppResources.Challenge1_ReduceSpeed);
                
                //Unable try; Go to Home
                _isRunning = false;
                if (_useEmulation)
                {
                    _speedEmulator.Stop();
                }else{
                    if (!_timer.IsEnabled) return;
                    _timer.Stop();
                    _seconds = _minTime;
                }
                _finish = true;

                var uri = new Uri("/BeatIt!;component/AppCode/Pages/home.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
            }
            else
            {
                if (_useEmulation)
                {
                    _speedEmulator.Start();
                }
                ShowTime.Text = _minTime.ToString(CultureInfo.InvariantCulture);
                ShowSpeed.Text = "0.00";

                StartRunningGrid.Visibility = Visibility.Collapsed;
                InProgressGrid.Visibility = Visibility.Visible;
            }
        }

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            _timer.Stop();
            if (_useEmulation)
            {
                _speedEmulator.Stop();
            }
            e.Cancel = false;
            base.OnBackKeyPress(e);
        }
    }

    internal class GpsSpeedEmulator
    {
        public delegate void EventSpeedChange(double s);

        private readonly FacadeController _ifc = FacadeController.GetInstance();

        private readonly Random _randomNumber;
        private readonly DispatcherTimer _timer;
        private readonly int _velocidadMinima;
        private int _mantenerVelocidadPor;
        private double _speed;
        private Estados _state = Estados.LlegarA10Km;

        public GpsSpeedEmulator()
        {
            var currentChallenge = (ChallengeDetail1)_ifc.GetChallenge(1);
            _velocidadMinima = currentChallenge.MinSpeed;
            _randomNumber = new Random();
            _mantenerVelocidadPor = 31;
            _timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            _timer.Tick += TickTimer;
        }

        public event EventSpeedChange SpeedChange;

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _speed = 0;
            _state = Estados.LlegarA10Km;
        }

        private void TickTimer(object o, EventArgs e)
        {
            if (!_timer.IsEnabled) return;
            int sumarRestar = _randomNumber.Next(0, 2);
            double dec = (1 + 0.0) / _randomNumber.Next(1, 11);

            if (_state == Estados.LlegarA10Km)
            {
                _speed += _randomNumber.Next(1, 4) + (sumarRestar - 1) * dec + sumarRestar * dec;
                if (_speed >= _velocidadMinima)
                {
                    _state = Estados.MantenerTiempo;
                }
            }
            else if (_state == Estados.MantenerTiempo && _mantenerVelocidadPor > 0)
            {
                _mantenerVelocidadPor--;
                double temp = _speed + (sumarRestar - 1) * dec + sumarRestar * dec;
                _speed += temp >= _velocidadMinima && temp < 24 ? temp - _speed : 0;

                if (_mantenerVelocidadPor == 0)
                {
                    _state = Estados.BajarVelocidad;
                }
            }
            else if (_state == Estados.BajarVelocidad)
            {
                _speed -= _randomNumber.Next(1, 4) - dec;
                if (_speed < _velocidadMinima)
                {
                    _speed = 0;
                    _timer.Stop();
                    _state = Estados.LlegarA10Km;
                }
            }
            SpeedChange(_speed);
        }

        private enum Estados
        {
            LlegarA10Km,
            MantenerTiempo,
            BajarVelocidad
        };
    }
}